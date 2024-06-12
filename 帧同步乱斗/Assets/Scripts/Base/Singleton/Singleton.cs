using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 单例基类
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class Singleton<T> where T : new()
{
    private static T instance;
    private static object mtx = new object();

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                lock (mtx)
                {
                    if (instance == null)
                    {
                        instance = new T();
                    }
                }
            }
            return instance;
        }

    }

    protected Singleton() { }
}
