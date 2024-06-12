using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IView
{
    protected IModel Data { get; set; }

    /// <summary>
    /// 绑定数据
    /// </summary>
    /// <param name="data"></param>
    public void BindData(IModel data)
    {
        UnbindData();
        Data = data;
        if (Data != null)
        {
            Data.ModifiedEvent += RefreshView;
        }


        RefreshView(data);
    }

    /// <summary>
    /// 解绑
    /// </summary>
    public void UnbindData()
    {
        if (Data != null)
        {
            Data.ModifiedEvent -= RefreshView;
        }
        Data = null;
    }

    /// <summary>
    /// 刷新视图
    /// </summary>
    /// <param name="data"></param>
    public void RefreshView(IModel data);
}
