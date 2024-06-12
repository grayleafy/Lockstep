

using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;



[Serializable]
public class ModelList
{
    [SerializeField]
    protected List<IModel> list = new List<IModel>();

    /// <summary>
    /// 计数改变事件
    /// </summary>
    public Action<ModelList> CountModifyEvent { get; set; }
    /// <summary>
    /// 元素更新事件
    /// </summary>
    public Action<ModelList, object> ItemModifyEvent { get; set; }

    public int Count { get { return list.Count; } }



    public void Add(IModel item)
    {
        list.Add(item);
        item.ModifiedEvent += OnItemModify;
        CountModifyEvent?.Invoke(this);
    }

    public void Remove(IModel item)
    {
        list.Remove(item);
        item.ModifiedEvent -= OnItemModify;
        CountModifyEvent?.Invoke(this);
    }

    public void Clear()
    {
        foreach (var model in list)
        {
            model.ModifiedEvent -= OnItemModify;
        }
        list.Clear();
    }

    private void OnItemModify(IModel item)
    {
        ItemModifyEvent?.Invoke(this, item);
    }

    public IModel this[int index]
    {
        get
        {
            return list[index];
        }
    }

    ~ModelList()
    {
        foreach (var item in list)
        {
            item.ModifiedEvent -= OnItemModify;
        }
    }
}


[Serializable]
public class ModelList<T> : ModelList where T : class, IModel
{
    /// <summary>
    /// 重置列表
    /// </summary>
    /// <param name="values"></param>
    public void Reset<V>(List<V> values, Func<V, T> transform)
    {
        Clear();
        for (int i = 0; i < values.Count; i++)
        {
            list.Add(transform(values[i]));
        }
        CountModifyEvent?.Invoke(this);
    }

    public void Add(T item)
    {
        base.Add(item);
    }

    public void Remove(T item)
    {
        base.Remove(item);
    }


    new public T this[int index]
    {
        get
        {
            return list[index] as T;
        }
    }


}

