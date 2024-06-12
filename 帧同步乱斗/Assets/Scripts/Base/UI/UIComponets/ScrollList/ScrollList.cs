using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


public class ScrollList : UIEffect
{
    [Header("使用注意事项：\n1.如果想要获取选择的物体序号，列表预制体需要包含Toggle组件\n2.注意content和item的锚点轴心设置")]
    [Space(32)]
    [SerializeField]
    private RectTransform content;
    [SerializeField]
    private RectTransform view;
    [SerializeField]
    private GridLayoutGroup gridLayoutGroup;
    [Header("配合Toggle使用")]
    public GameObject prefab;
    private ModelList datas = new();

    private List<RectTransform> freeItems = new List<RectTransform>();                        //空余格子
    private Dictionary<int, RectTransform> showItems = new Dictionary<int, RectTransform>();  //正在显示的格子
    private Vector3 contentLastPos = Vector3.zero;                                            //content上一帧的位置
    private Dictionary<Toggle, UnityAction<bool>> buttonListeners = new();                     //暂时缓冲item按钮添加的事件监听

    //当前选中的item
    public int currentSelectedIndex = -1;
    #region 可监听事件
    public Action<int> deselectEvent;                                   //外部添加监听，当选择的视图改变时，调用此事件
    public Action<int> selectEvent;                                     //外部添加监听，当选择的视图改变时，调用此事件
    #endregion

    #region 生命周期
    public override void Start()
    {
        base.Start();

        //布局禁用
        gridLayoutGroup.enabled = false;

        OnRefresh();
    }

    public override void Update()
    {
        base.Update();
        //检测content是否移动
        if (content.position != contentLastPos)
        {
            OnRectMove();
        }
        contentLastPos = content.position;
    }

    void OnDestroy()
    {
        UnbindData();
    }
    #endregion


    #region 外部接口
    /// <summary>
    /// 绑定数据
    /// </summary>
    /// <param name="datas"></param>
    public void BindData<T>(ModelList<T> datas) where T : class, IDisplayableModel
    {
        UnbindData();
        this.datas = datas;

        //绑定事件
        datas.CountModifyEvent += OnRefresh;
        //datas.ItemModifyEvent += OnItemModidy;

        //立即刷新
        OnRefresh();
    }

    public void UnbindData()
    {
        if (datas != null)
        {
            datas.CountModifyEvent -= OnRefresh;
            //datas.ItemModifyEvent -= OnItemModidy;
        }
        datas = new ModelList();

        OnRefresh();
    }
    #endregion




    #region 主要更新阶段


    /// <summary>
    /// 刷新
    /// </summary>
    public void OnRefresh(ModelList datas = null)
    {
        //取消选择事件
        if (currentSelectedIndex != -1)
        {
            deselectEvent?.Invoke(currentSelectedIndex);
        }
        currentSelectedIndex = -1;

        //扩展content
        ExtendContent();
        //移除所有已经存在的元素
        var cellIndexs = showItems.Keys.ToArray();
        foreach (var index in cellIndexs)
        {
            HideItem(index);
        }
        //重新计算需要的元素并且加载
        OnRectMove();
    }

    //单个元素改变，重新绘制它的格子内容，不包括位置
    private void OnItemModidy(ModelList<IDisplayableModel> list, IDisplayableModel item)
    {
        //先得到索引
        int index = 0;
        for (int i = 0; i < list.Count; i++)
        {
            if (item == list[i])
            {
                break;
            }
        }

        //如果正在显示,重新绘制
        if (showItems.ContainsKey(index))
        {
            GetView(showItems[index]);
        }
    }

    //滚动区域移动区域移动，更新显示和隐藏
    private void OnRectMove()
    {
        List<int> needShowIndexs = GetNeedShowIndex();
        ShowItems(needShowIndexs);
        RemoveUnNeedItem(needShowIndexs);
    }

    //显示一个新的item
    void ShowItem(RectTransform item, int index)
    {
        SetItemPos(item, index);
        IView view = GetView(item);
        view.BindData(datas[index]);

        //按钮事件添加
        Toggle toggle = (view as MonoBehaviour).GetComponent<Toggle>();
        if (toggle != null)
        {
            UnityAction<bool> action = (value) =>
            {
                if (value == false && currentSelectedIndex == index)
                {
                    toggle.isOn = true;
                }

                if (value)
                {
                    //未变化
                    if (index == currentSelectedIndex)
                    {
                        return;
                    }
                    int lastIndex = currentSelectedIndex;
                    currentSelectedIndex = index;
                    if (lastIndex != -1)
                    {
                        //正在显示则关闭
                        if (showItems.ContainsKey(lastIndex))
                        {
                            showItems[lastIndex].GetComponent<Toggle>().isOn = false;
                        }
                        deselectEvent?.Invoke(lastIndex);
                    }
                    selectEvent?.Invoke(currentSelectedIndex);
                }
            };
            buttonListeners.Add(toggle, action);
            toggle.onValueChanged.AddListener(action);


            if (index == currentSelectedIndex)
            {
                toggle.isOn = true;
            }
            else
            {
                toggle.isOn = false;
            }
        }
    }

    //隐藏一个item
    void HideItem(int index)
    {
        showItems[index].gameObject.SetActive(false);
        freeItems.Add(showItems[index]);
        IView view = GetView(showItems[index]);
        view.UnbindData();
        showItems.Remove(index);

        Toggle toggle = (view as MonoBehaviour).GetComponent<Toggle>();
        if (toggle != null && buttonListeners.ContainsKey(toggle))
        {
            toggle.onValueChanged.RemoveListener(buttonListeners[toggle]);
            buttonListeners.Remove(toggle);
        }

        if (toggle != null)
        {
            toggle.isOn = false;
        }

    }

    #endregion


    #region 实现方法

    //按钮点击更新事件
    void ButtonClick()
    {

    }

    /// <summary>
    /// 获取视图
    /// </summary>
    /// <param name="cell"></param>
    /// <returns></returns>
    IView GetView(RectTransform cell)
    {
        return cell.GetComponent<IView>();
    }

    //根据数据数量，重新计算content区域的大小
    void ExtendContent()
    {
        //竖直扩展
        if (gridLayoutGroup.startAxis == GridLayoutGroup.Axis.Horizontal)
        {
            int colCount = GetNumByLength(content.Width(), gridLayoutGroup.cellSize.x, gridLayoutGroup.spacing.x);
            colCount = Mathf.Max(1, colCount);
            int lineCount = (datas.Count + colCount - 1) / colCount;
            float height = GetLengthByNum(lineCount, gridLayoutGroup.cellSize.y, gridLayoutGroup.spacing.y);
            content.SetSize(new Vector2(content.Width(), height), gridLayoutGroup.startCorner);
        }
        //水平扩展
        else
        {
            int lineCount = GetNumByLength(content.Height(), gridLayoutGroup.cellSize.y, gridLayoutGroup.spacing.y);
            lineCount = Mathf.Max(1, lineCount);
            int colCount = (datas.Count + lineCount - 1) / lineCount;
            float width = GetLengthByNum(colCount, gridLayoutGroup.cellSize.x, gridLayoutGroup.spacing.x);
            content.SetSize(new Vector2(width, content.Height()), gridLayoutGroup.startCorner);
        }
    }

    //给定长度能容纳的物体数量
    int GetNumByLength(float length, float itemSize, float interval)
    {
        return (int)((length + interval) / (itemSize + interval));
    }

    //根据数量计算占用的最小长度
    float GetLengthByNum(int count, float itemSize, float interval)
    {
        return count * itemSize + Mathf.Max(0, count - 1) * interval;
    }

    //格子是否离开视图
    bool IsOutView(RectTransform cell)
    {
        Vector3[] cellCorners = new Vector3[4];
        cell.GetWorldCorners(cellCorners);

        Vector3[] viewCorners = new Vector3[4];
        view.GetWorldCorners(viewCorners);

        return (cellCorners[0].x > viewCorners[2].x || cellCorners[2].x < viewCorners[0].x) && (cellCorners[0].y > viewCorners[2].y || cellCorners[2].y < viewCorners[0].y);
    }

    //移除不需要的格子，加入空闲池
    void RemoveUnNeedItem(List<int> needShowIndexs)
    {
        HashSet<int> hash = needShowIndexs.ToHashSet();

        var cellIndexs = showItems.Keys.ToArray();
        foreach (var index in cellIndexs)
        {
            if (hash.Contains(index) == false)
            {
                HideItem(index);
            }
        }
    }


    //计算显示区域需要的数据下标
    List<int> GetNeedShowIndex()
    {
        GetRange(content, view, gridLayoutGroup.startCorner, out Vector2 min, out Vector2 max);

        GetIndexRange(min.x, max.x, gridLayoutGroup.cellSize.x, gridLayoutGroup.spacing.x, out int xl, out int xr);
        GetIndexRange(min.y, max.y, gridLayoutGroup.cellSize.y, gridLayoutGroup.spacing.y, out int yl, out int yr);

        List<int> res = new();

        if (gridLayoutGroup.startAxis == GridLayoutGroup.Axis.Horizontal)
        {
            int colCount = GetNumByLength(content.Width(), gridLayoutGroup.cellSize.x, gridLayoutGroup.spacing.x);
            colCount = Mathf.Max(colCount, 1);
            xl = Mathf.Max(xl, 0);
            xr = Mathf.Min(xr, colCount);
            for (int y = yl; y < yr; y++)
            {
                for (int x = xl; x < xr; x++)
                {
                    res.Add(y * colCount + x);
                }
            }
        }
        else
        {
            int lineCount = GetNumByLength(content.Height(), gridLayoutGroup.cellSize.y, gridLayoutGroup.spacing.y);
            lineCount = Mathf.Max(lineCount, 1);
            yl = Mathf.Max(yl, 0);
            yr = Mathf.Min(yr, lineCount);
            for (int x = xl; x < xr; x++)
            {
                for (int y = yl; y < yr; y++)
                {
                    res.Add(x * lineCount + y);
                }
            }
        }

        return res;
    }

    //计算从起点角的距离
    void GetRange(RectTransform p, RectTransform c, GridLayoutGroup.Corner startCorner, out Vector2 min, out Vector2 max)
    {
        Vector3[] pCorners = new Vector3[4];
        Vector3[] cCorners = new Vector3[4];
        p.GetWorldCorners(pCorners);
        c.GetWorldCorners(cCorners);
        Vector3 start = new Vector3();
        Vector3 minCorner = Vector3.zero;
        Vector3 maxCorner = Vector3.zero;
        if (startCorner == GridLayoutGroup.Corner.LowerLeft)
        {
            start = pCorners[0];
            minCorner = cCorners[0];
            maxCorner = cCorners[2];
        }
        else if (startCorner == GridLayoutGroup.Corner.UpperLeft)
        {
            start = pCorners[1];
            minCorner = cCorners[1];
            maxCorner = cCorners[3];
        }
        else if (startCorner == GridLayoutGroup.Corner.UpperRight)
        {
            start = pCorners[2];
            minCorner = cCorners[2];
            maxCorner = cCorners[0];
        }
        else if (startCorner == GridLayoutGroup.Corner.LowerRight)
        {
            start = pCorners[3];
            minCorner = cCorners[3];
            maxCorner = cCorners[1];
        }

        min = minCorner - start;
        max = maxCorner - start;

        if (startCorner == GridLayoutGroup.Corner.LowerRight || startCorner == GridLayoutGroup.Corner.UpperRight)
        {
            min.x = -min.x;
            max.x = -max.x;
        }

        if (startCorner == GridLayoutGroup.Corner.UpperLeft || startCorner == GridLayoutGroup.Corner.UpperLeft)
        {
            min.y = -min.y;
            max.y = -max.y;
        }
    }

    void GetIndexRange(float min, float max, float cell, float space, out int l, out int r)
    {
        l = (int)Mathf.Floor((min + space) / (cell + space));
        r = (int)Mathf.Ceil((max) / (cell + space));
    }

    //显示需要显示的物体
    void ShowItems(List<int> needShowIndexs)
    {
        for (int i = 0; i < needShowIndexs.Count; i++)
        {
            //合法性判断
            if (needShowIndexs[i] < 0 || needShowIndexs[i] >= datas.Count)
            {
                continue;
            }

            //已经存在
            if (showItems.ContainsKey(needShowIndexs[i]))
            {
                continue;
            }
            else
            {
                //取一个对象
                RectTransform item;
                if (freeItems.Count > 0)
                {
                    item = freeItems[freeItems.Count - 1];
                    item.gameObject.SetActive(true);
                    freeItems.RemoveAt(freeItems.Count - 1);
                }
                else
                {
                    item = GameObject.Instantiate(prefab, gridLayoutGroup.transform, false).transform as RectTransform;
                }
                showItems.Add(needShowIndexs[i], item);

                //设置item
                ShowItem(item, needShowIndexs[i]);
            }
        }
    }



    //设置item位置
    void SetItemPos(RectTransform item, int index)
    {
        int x, y;
        if (gridLayoutGroup.startAxis == GridLayoutGroup.Axis.Horizontal)
        {
            int colCount = GetNumByLength(content.Width(), gridLayoutGroup.cellSize.x, gridLayoutGroup.spacing.x);
            colCount = Mathf.Max(colCount, 1);
            y = index / colCount;
            x = index % colCount;
        }
        else
        {
            int lineCount = GetNumByLength(content.Height(), gridLayoutGroup.cellSize.y, gridLayoutGroup.spacing.y);
            lineCount = Mathf.Max(lineCount, 1);
            x = index / lineCount;
            y = index % lineCount;
        }

        Vector2 delta = Vector2.zero;
        delta.x = (gridLayoutGroup.cellSize.x + gridLayoutGroup.spacing.x) * x;
        delta.y = (gridLayoutGroup.cellSize.y + gridLayoutGroup.spacing.y) * y;
        if (gridLayoutGroup.startCorner == GridLayoutGroup.Corner.LowerRight || gridLayoutGroup.startCorner == GridLayoutGroup.Corner.UpperRight)
        {
            delta.x = -delta.x;
        }
        if (gridLayoutGroup.startCorner == GridLayoutGroup.Corner.UpperLeft || gridLayoutGroup.startCorner == GridLayoutGroup.Corner.UpperRight)
        {
            delta.y = -delta.y;
        }

        item.SetCornerPos(content.position + new Vector3(delta.x, delta.y, 0), gridLayoutGroup.startCorner);
    }

    #endregion
}
