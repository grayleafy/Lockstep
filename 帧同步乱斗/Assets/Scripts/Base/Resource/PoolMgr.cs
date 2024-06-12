using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 抽屉数据  池子中的一列容器
/// </summary>
public class PoolData
{
    //抽屉中 对象挂载的父节点
    public GameObject fatherObj;
    //对象的容器
    public List<GameObject> poolList;

    public PoolData(GameObject obj, GameObject poolObj)
    {
        //给我们的抽屉 创建一个父对象 并且把他作为我们pool(衣柜)对象的子物体
        fatherObj = new GameObject(obj.name);
        fatherObj.transform.parent = poolObj.transform;
        poolList = new List<GameObject>() { };
        PushObj(obj);
    }

    /// <summary>
    /// 往抽屉里面 压都东西
    /// </summary>
    /// <param name="obj"></param>
    public void PushObj(GameObject obj)
    {
        //失活 让其隐藏
        obj.SetActive(false);
        //存起来
        poolList.Add(obj);
        //设置父对象
        obj.transform.parent = fatherObj.transform;
    }

    /// <summary>
    /// 从抽屉里面 取东西
    /// </summary>
    /// <returns></returns>
    public GameObject GetObj()
    {
        GameObject obj = null;
        //取出最后一个
        int count = poolList.Count;
        obj = poolList[count - 1];
        poolList.RemoveAt(count - 1);
        //激活 让其显示
        obj.SetActive(true);
        //断开了父子关系
        obj.transform.parent = null;

        return obj;
    }
}

/// <summary>
/// GameObject缓存池模块
/// </summary>
public class PoolMgr : Singleton<PoolMgr>
{
    //缓存池容器 （衣柜）
    public Dictionary<string, PoolData> poolDic = new Dictionary<string, PoolData>();

    private GameObject poolObj;

    /// <summary>
    /// 往外拿东西
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public void GetObj(string abName, string name, UnityAction<GameObject> callBack)
    {
        string fullName = abName + "." + name;
        //有抽屉 并且抽屉里有东西
        if (poolDic.ContainsKey(fullName) && poolDic[fullName].poolList.Count > 0)
        {
            callBack(poolDic[fullName].GetObj());
        }
        else
        {
            //通过异步加载资源 创建对象给外部用
            ABMgr.Instance.LoadResAsync(abName, name, (o) =>
            {
                GameObject go = o as GameObject;
                go = GameObject.Instantiate(go);
                go.name = name;
                callBack(go);
            });

            //obj = GameObject.Instantiate(Resources.Load<GameObject>(name));
            //把对象名字改的和池子名字一样
            //obj.name = name;
        }
    }

    /// <summary>
    /// 换暂时不用的东西给我
    /// </summary>
    public void PushObj(string abName, string name, GameObject obj)
    {
        string fullName = abName + "." + name;
        if (poolObj == null)
            poolObj = new GameObject("Pool");

        //里面有抽屉
        if (poolDic.ContainsKey(fullName))
        {
            poolDic[fullName].PushObj(obj);
        }
        //里面没有抽屉
        else
        {
            poolDic.Add(fullName, new PoolData(obj, poolObj));
        }
    }

    /// <summary>
    /// 是否存在某个物体
    /// </summary>
    /// <param name="abName"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public bool ContainObj(string abName, string name)
    {
        string fullName = abName + "." + name;
        //有抽屉 并且抽屉里有东西
        if (poolDic.ContainsKey(fullName) && poolDic[fullName].poolList.Count > 0)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// 清空缓存池的方法 
    /// 主要用在 场景切换时
    /// </summary>
    public void Clear()
    {
        poolDic.Clear();
        poolObj = null;
    }
}