using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TimeMgr : SingletonMono<TimeMgr>
{
    public float lastRealTime = 0;
    public float realTimeDelta = 0;
    public float monoTimeDelta { get { return Time.deltaTime; } }


    //定时器相关
    private SortedDictionary<float, Action> tasks = new();

    private void Start()
    {
        lastRealTime = Time.realtimeSinceStartup;
    }

    private void Update()
    {
        //时间delta更新
        float temp = Time.realtimeSinceStartup;
        realTimeDelta = temp - lastRealTime;
        lastRealTime = temp;

        //定时器更新
        while (tasks.Count > 0 && tasks.First().Key <= Time.realtimeSinceStartup)
        {
            tasks.First().Value.Invoke();
            tasks.Remove(tasks.First().Key);
        }
    }

    /// <summary>
    /// 延时调用，现实延迟时间
    /// </summary>
    /// <param name="delay"></param>
    /// <param name="action"></param>
    public void DelayCallForRealTime(float delay, Action action)
    {
        tasks.Add(Time.realtimeSinceStartup + delay, action);
    }
}
