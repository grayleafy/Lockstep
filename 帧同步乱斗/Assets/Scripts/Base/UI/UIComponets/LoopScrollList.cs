using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;



public class LoopScrollList : UIEffect, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    [Header("注意事项\ncontent和item的轴心需要为正中心")]
    //数据
    private ModelList _data = new();

    //设置
    public enum ArrangementEnum
    {
        FromLeftToRight,
        FromRightToLeft,
        FromUpperToLower,
        FromLowerToUpper,
    }
    [SerializeField, Header("排列方式")]
    private ArrangementEnum arrangement = ArrangementEnum.FromLeftToRight;
    [SerializeField, Header("物体内容")]
    private RectTransform content;
    [SerializeField, Header("物体预制体")]
    private GameObject itemPrefab;
    [SerializeField, Header("格子大小")]
    private Vector2 cellSize = new Vector2(100, 100);
    [SerializeField, Header("格子间距")]
    private Vector2 cellSpace = new Vector2(0, 0);

    [Header("调整速度,每秒移动的距离")]
    public float adjustSpeed = 400;
    [Header("大小随着离中心的距离变化曲线,横坐标1表示消失时的距离")]
    public AnimationCurve sizeCurve = new AnimationCurve(new Keyframe[] { new Keyframe(0, 1), new Keyframe(1, 0.5f) });

    //运行时维护的数据
    private List<RectTransform> freeItems = new List<RectTransform>();                          //空余格子
    private Dictionary<int, RectTransform> showItems = new Dictionary<int, RectTransform>();    //正在显示的格子
    [SerializeField, Header("视图偏移量")]
    private Vector2 viewPrivotPos = Vector2.zero;                                               //视口的轴心位置
    [SerializeField, Header("调整偏移量")]
    private Vector2 adjustDelta = Vector2.zero;                                                 //调整偏移量
    private bool isDraging = false;                                                             //是否正在拖拽
    [SerializeField]
    private int currentSelectIndex = 0;                                                         //当前选择的下标
    /// <summary>
    /// 当前选择的真实下标
    /// </summary>
    public int CurrentSelectRealIndex
    {
        get
        {
            if (_data.Count == 0)
            {
                return -1;
            }
            return (currentSelectIndex % _data.Count + _data.Count) % _data.Count;
        }
    }

    /// <summary>
    /// 事件  <旧下标，新下标>
    /// </summary>
    public Action<int, int> SelectModifiedEvent;                                                     //选择的元素切换事件

    #region 生命周期
    public void OnBeginDrag(PointerEventData eventData)
    {
        isDraging = true;

        //取消调整
        adjustDelta = Vector2.zero;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 delta = Vector2.zero;

        //水平方向
        if (arrangement == ArrangementEnum.FromLeftToRight || arrangement == ArrangementEnum.FromRightToLeft)
        {
            delta.x += eventData.delta.x;
        }
        //垂直方向
        if (arrangement == ArrangementEnum.FromUpperToLower || arrangement == ArrangementEnum.FromLowerToUpper)
        {
            delta.y += eventData.delta.y;
        }



        //移动并且计算显隐
        OnRectMove(delta);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDraging = false;
        //计算下标
        CalculateSelectIndex();
        //朝最近的下标移动调整
        adjustDelta = GetAdjustDelta();
    }

    public override void Update()
    {
        base.Update();
        if (isDraging == false)
        {
            Adjust();
        }
    }
    #endregion

    #region 数据绑定
    /// <summary>
    /// 绑定数据
    /// </summary>
    /// <param name="datas"></param>
    public void BindData<T>(ModelList<T> datas) where T : class, IModel
    {
        UnbindData();
        _data = datas;

        //绑定事件
        datas.CountModifyEvent += OnRefresh;
        //datas.ItemModifyEvent += OnItemModidy;

        //立即刷新
        OnRefresh();
    }
    /// <summary>
    /// 解除数据绑定
    /// </summary>
    public void UnbindData()
    {
        if (_data != null)
        {
            _data.CountModifyEvent -= OnRefresh;
            //datas.ItemModifyEvent -= OnItemModidy;
        }
        _data = new ModelList();

        OnRefresh();
    }
    /// <summary>
    /// 设置选择的下标，UI立即切换
    /// </summary>
    /// <param name="index"></param>
    public void SetSelectIndexImmediately(int index)
    {
        if (index % _data.Count != currentSelectIndex % _data.Count)
        {
            SelectModifiedEvent?.Invoke(GetReallyIndex(currentSelectIndex), GetReallyIndex(index));
        }
        currentSelectIndex = index;

        Vector2 delta = GetAdjustDelta();
        OnRectMove(delta);
    }

    /// <summary>
    /// 设置选择的下标，UI缓慢切换
    /// </summary>
    /// <param name="index"></param>
    /// <param name="moreOrless">优先移动的方向</param>
    public void SetSelectIndexSlowly(int index, int moreOrless)
    {
        //计算最近的index
        if (moreOrless > 0)
        {
            while (index < currentSelectIndex)
            {
                index += _data.Count;
            }
            while (index - _data.Count >= currentSelectIndex)
            {
                index -= _data.Count;
            }
        }
        else if (moreOrless < 0)
        {
            while (index > currentSelectIndex)
            {
                index -= _data.Count;
            }
            while (index + _data.Count <= currentSelectIndex)
            {
                index += _data.Count;
            }
        }


        if (index % _data.Count != currentSelectIndex % _data.Count)
        {
            SelectModifiedEvent?.Invoke(GetReallyIndex(currentSelectIndex), GetReallyIndex(index));
        }
        currentSelectIndex = index;

        adjustDelta = GetAdjustDelta();
    }
    #endregion

    #region 关键方法
    //刷新
    void OnRefresh(ModelList list = null)
    {
        //取消调整
        adjustDelta = Vector2.zero;

        //还原
        viewPrivotPos = Vector2.zero;

        //移除所有显示的元素
        var cellIndexs = showItems.Keys.ToArray();
        foreach (var index in cellIndexs)
        {
            HideItem(index);
        }

        //下标
        currentSelectIndex = 0;

        //移动更新
        OnRectMove(Vector2.zero);

        //更新选中的下标
        CalculateSelectIndex();
    }
    //移动
    void OnRectMove(Vector2 delta)
    {
        viewPrivotPos += delta;
        //整数倍转化，避免数值过大
        Mod();
        MoveItems(delta);
        var needIndexs = GetNeedShowIndex();
        HideUnneedItems(needIndexs);
        if (_data.Count > 0) ShowNeedItems(needIndexs);
        SetItemSize();
    }


    //隐藏单个元素
    void HideItem(int index)
    {
        showItems[index].gameObject.SetActive(false);
        freeItems.Add(showItems[index]);
        IView view = GetView(showItems[index]);
        view.UnbindData();
        showItems.Remove(index);
    }
    //显示一个元素
    void ShowItem(int index, RectTransform rect)
    {
        showItems.Add(index, rect);
        SetItemPos(index, rect);
        IView view = GetView(rect);
        view.BindData(_data[GetReallyIndex(index)]);
    }
    //计算选择的下标并触发事件
    void CalculateSelectIndex()
    {
        if (_data.Count == 0)
        {
            currentSelectIndex = 0;
            return;
        }

        Vector2 localPos = -viewPrivotPos;
        int index = 0;
        if (arrangement == ArrangementEnum.FromLeftToRight)
        {
            index = Mathf.RoundToInt(localPos.x / (cellSize.x + cellSpace.x));
        }
        else if (arrangement == ArrangementEnum.FromRightToLeft)
        {
            index = Mathf.RoundToInt(-localPos.x / (cellSize.x + cellSpace.x));
        }
        if (arrangement == ArrangementEnum.FromLowerToUpper)
        {
            index = Mathf.RoundToInt(localPos.y / (cellSize.y + cellSpace.y));
        }
        else if (arrangement == ArrangementEnum.FromUpperToLower)
        {
            index = Mathf.RoundToInt(-localPos.y / (cellSize.y + cellSpace.y));
        }

        if (index % _data.Count != currentSelectIndex % _data.Count)
        {
            SelectModifiedEvent?.Invoke(GetReallyIndex(currentSelectIndex), GetReallyIndex(index));
        }
        currentSelectIndex = index;
    }

    #endregion

    //一次求余操作
    void Mod()
    {
        if (_data.Count == 0)
        {
            viewPrivotPos = Vector2.zero;
            currentSelectIndex = 0;
            return;
        }

        if (arrangement == ArrangementEnum.FromLeftToRight || arrangement == ArrangementEnum.FromRightToLeft)
        {
            while (viewPrivotPos.x < -_data.Count * (cellSize.x + cellSpace.x))
            {
                viewPrivotPos.x += _data.Count * (cellSize.x + cellSpace.x);
                ModItemIndex(arrangement == ArrangementEnum.FromLeftToRight ? -_data.Count : _data.Count);
            }
            while (viewPrivotPos.x > _data.Count * (cellSize.x + cellSpace.x))
            {
                viewPrivotPos.x -= _data.Count * (cellSize.x + cellSpace.x);
                ModItemIndex(arrangement == ArrangementEnum.FromLeftToRight ? _data.Count : -_data.Count);
            }
        }
        else
        {
            while (viewPrivotPos.y < -_data.Count * (cellSize.y + cellSpace.y))
            {
                viewPrivotPos.y += _data.Count * (cellSize.y + cellSpace.y);
                ModItemIndex(arrangement == ArrangementEnum.FromLowerToUpper ? -_data.Count : _data.Count);
            }
            while (viewPrivotPos.y > _data.Count * (cellSize.y + cellSpace.y))
            {
                viewPrivotPos.y -= _data.Count * (cellSize.y + cellSpace.y);
                ModItemIndex(arrangement == ArrangementEnum.FromLowerToUpper ? _data.Count : -_data.Count);
            }
        }
    }
    //取模后的真实下标
    int GetReallyIndex(int index)
    {
        return (index % _data.Count + _data.Count) % _data.Count;
    }

    // 获取视图
    IView GetView(RectTransform cell)
    {
        return cell.GetComponent<IView>();
    }
    //已经显示的元素的下标进行偏移
    void ModItemIndex(int offset)
    {
        Dictionary<int, RectTransform> temp = new();
        foreach (int index in showItems.Keys)
        {
            temp.Add(index, showItems[index]);
        }
        showItems.Clear();
        foreach (int index in temp.Keys)
        {
            showItems.Add(index + offset, temp[index]);
        }

        currentSelectIndex += offset;
    }

    //计算调整偏移量,移动到整数倍位置
    Vector2 GetAdjustDelta()
    {
        Vector2 endPos = Vector2.zero;
        int index = currentSelectIndex;
        if (arrangement == ArrangementEnum.FromLeftToRight)
        {
            endPos.x = index * (cellSize.x + cellSpace.x);
        }
        else if (arrangement == ArrangementEnum.FromRightToLeft)
        {
            endPos.x = -index * (cellSize.x + cellSpace.x);
        }
        if (arrangement == ArrangementEnum.FromLowerToUpper)
        {
            endPos.y = index * (cellSize.y + cellSpace.y);
        }
        else if (arrangement == ArrangementEnum.FromUpperToLower)
        {
            endPos.y = -index * (cellSize.y + cellSpace.y);
        }

        return -endPos - viewPrivotPos;
    }
    //update中调整元素的位置
    void Adjust()
    {
        if (adjustDelta != Vector2.zero)
        {
            //如果是最后一帧
            if (Mathf.Abs(adjustDelta.magnitude) <= adjustSpeed * TimeMgr.Instance.realTimeDelta)
            {
                OnRectMove(adjustDelta);
                adjustDelta = Vector2.zero;
            }
            //否则按速度调整
            else
            {
                if (adjustDelta.x > 0)
                {
                    adjustDelta.x -= adjustSpeed * TimeMgr.Instance.realTimeDelta;
                    OnRectMove(new Vector2(adjustSpeed * TimeMgr.Instance.realTimeDelta, 0));
                }
                else if (adjustDelta.x < 0)
                {
                    adjustDelta.x -= -adjustSpeed * TimeMgr.Instance.realTimeDelta;
                    OnRectMove(new Vector2(-adjustSpeed * TimeMgr.Instance.realTimeDelta, 0));
                }
                if (adjustDelta.y > 0)
                {
                    adjustDelta.y -= adjustSpeed * TimeMgr.Instance.realTimeDelta;
                    OnRectMove(new Vector2(0, adjustSpeed * TimeMgr.Instance.realTimeDelta));
                }
                else if (adjustDelta.y < 0)
                {
                    adjustDelta.y -= -adjustSpeed * TimeMgr.Instance.realTimeDelta;
                    OnRectMove(new Vector2(0, -adjustSpeed * TimeMgr.Instance.realTimeDelta));
                }
            }

        }
    }


    //移动所有item
    void MoveItems(Vector2 delta)
    {
        foreach (var rect in showItems.Values)
        {
            rect.position += new Vector3(delta.x, delta.y, 0);
        }
    }

    void SetItemSize()
    {
        foreach (var rect in showItems.Values)
        {
            //设置大小
            float distance = Mathf.Abs((rect.position - content.position).magnitude);
            float maxDis;
            if (arrangement == ArrangementEnum.FromLeftToRight || arrangement == ArrangementEnum.FromRightToLeft)
            {
                maxDis = content.rect.width / 2 + cellSize.x / 2;
            }
            else
            {
                maxDis = content.rect.height / 2 + cellSize.y / 2;
            }
            float size = sizeCurve.Evaluate(distance / maxDis);
            rect.localScale = new Vector3(size, size, 1);
        }
    }

    //计算需要显示的物体下标
    HashSet<int> GetNeedShowIndex()
    {
        int si = 0, ti = 0;
        float rectl = -viewPrivotPos.x - content.rect.width / 2;
        float rectr = -viewPrivotPos.x + content.rect.width / 2;
        float rectu = -viewPrivotPos.y + content.rect.height / 2;
        float rectd = -viewPrivotPos.y - content.rect.height / 2;
        if (arrangement == ArrangementEnum.FromLeftToRight)
        {
            GetIndexRange(rectl, rectr, cellSize.x, cellSpace.x, out si, out ti);
        }
        else if (arrangement == ArrangementEnum.FromRightToLeft)
        {
            GetIndexRange(-rectr, -rectl, cellSize.x, cellSpace.x, out si, out ti);
        }
        else if (arrangement == ArrangementEnum.FromLowerToUpper)
        {
            GetIndexRange(rectd, rectu, cellSize.y, cellSpace.y, out si, out ti);
        }
        else if (arrangement == ArrangementEnum.FromUpperToLower)
        {
            GetIndexRange(-rectu, -rectd, cellSize.y, cellSpace.y, out si, out ti);
        }

        HashSet<int> res = new();
        for (int i = si; i < ti; i++)
        {
            res.Add(i);
        }
        return res;
    }

    void GetIndexRange(float s, float t, float cellSize, float cellSpace, out int si, out int ti)
    {
        si = (int)Mathf.Floor((s + cellSize / 2 + cellSpace) / (cellSize + cellSpace));
        ti = (int)Mathf.Ceil((t + cellSize / 2) / (cellSize + cellSpace));
    }
    //隐藏不需要的元素
    void HideUnneedItems(HashSet<int> needIndex)
    {
        var cellIndexs = showItems.Keys.ToArray();
        foreach (var index in cellIndexs)
        {
            if (needIndex.Contains(index) == false)
            {
                HideItem(index);
            }
        }
    }
    //显示需要的元素
    void ShowNeedItems(HashSet<int> needIndex)
    {
        foreach (int index in needIndex)
        {
            //已经存在
            if (showItems.ContainsKey(index) == true)
            {
                continue;
            }

            RectTransform rect;
            if (freeItems.Count > 0)
            {
                rect = freeItems[freeItems.Count - 1];
                rect.gameObject.SetActive(true);
                freeItems.RemoveAt(freeItems.Count - 1);
            }
            else
            {
                rect = GameObject.Instantiate(itemPrefab, content, false).transform as RectTransform;
            }
            ShowItem(index, rect);
        }
    }

    //设置新显示的元素的位置
    void SetItemPos(int index, RectTransform rect)
    {
        Vector2 localPos = Vector2.zero;
        if (arrangement == ArrangementEnum.FromLeftToRight)
        {
            localPos.x = index * (cellSize.x + cellSpace.x);
        }
        else if (arrangement == ArrangementEnum.FromRightToLeft)
        {
            localPos.x = -index * (cellSize.x + cellSpace.x);
        }
        else if (arrangement == ArrangementEnum.FromLowerToUpper)
        {
            localPos.y = index * (cellSize.y + cellSpace.y);
        }
        else if (arrangement == ArrangementEnum.FromUpperToLower)
        {
            localPos.y = -index * (cellSize.y + cellSpace.y);
        }

        rect.position = content.position + new Vector3(viewPrivotPos.x, viewPrivotPos.y, 0) + new Vector3(localPos.x, localPos.y, 0);
    }
}
