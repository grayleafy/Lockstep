
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 共享变量黑板
/// </summary>
[Serializable]
public class InspectorBlackboard
{
    [SerializeField]
    private SerializableDictionaryFT<string, InspectorBlackboardValue> dic = new();    //共享黑板


    /// <summary>
    /// 设置一个黑板变量
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="name"></param>
    /// <param name="value"></param>
    public void SetValue<T>(string name, T value) where T : InspectorBlackboardValue
    {
        if (dic.ContainsKey(name))
        {
            dic[name] = value;
        }
        else
        {
            dic.Add(name, value);
        }
    }

    /// <summary>
    /// 获取一个黑板变量，如果是值类型并且不存在则返回默认值
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="name"></param>
    /// <returns></returns>
    public T GetValue<T>(string name) where T : InspectorBlackboardValue
    {
        if (dic.ContainsKey(name))
        {
            return (T)dic[name];
        }
        else
        {
            return default(T);
        }
    }
}
