using FixMath.NET;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogicFrameMgr : Singleton<LogicFrameMgr>
{
    private Queue<int> freeIds = new Queue<int>();
    public Dictionary<int, LogicEntity> allLogicEntities = new Dictionary<int, LogicEntity>();

    public LogicFrameMgr()
    {
        for (int i = 0; i < 10000; i++)
        {
            freeIds.Enqueue(i);
        }
    }

    public LogicEntity GetLogicEntity(int id)
    {
        return allLogicEntities[id];
    }



    /// <summary>
    /// 逻辑实体注册，每一个实体创建时使用
    /// </summary>
    /// <param name="entity"></param>
    public void LogicEntityLogin(LogicEntity entity)
    {
        entity.id = GetId();
        if (allLogicEntities.ContainsKey(entity.id))
        {
            Logger.Instance.LogError(LogType.LogicFrame, "id重复");
        }
        allLogicEntities.Add(entity.id, entity);
        Logger.Instance.Log(LogType.LogicEntity, "逻辑实体注册 id:" + entity.id);
    }
    /// <summary>
    /// 逻辑实体注销，每一个实体销毁时使用
    /// </summary>
    /// <param name="entity"></param>
    public void LogicEntityLogout(LogicEntity entity)
    {
        PushId(entity.id);
        if (allLogicEntities.ContainsKey(entity.id) == false)
        {
            Logger.Instance.LogError(LogType.LogicFrame, "注销一个不存在id的逻辑实体");
        }
        allLogicEntities.Remove(entity.id);
        Logger.Instance.Log(LogType.LogicEntity, "逻辑实体注销 id:" + entity.id);
    }
    /// <summary>
    /// 逻辑帧更新
    /// </summary>
    /// <param name="dt"></param>
    public void LogicFrameUpdate(Fix64 dt)
    {
        PhysicsMgr.Instance.OnLogicUpdate();
        foreach (var entity in allLogicEntities.Values)
        {
            if (entity.enabled)
            {
                entity.OnLogicFrameUpdate(dt);
            }
        }
    }










    #region 工具函数
    //获取空闲id
    int GetId()
    {
        if (freeIds.Count > 0)
        {
            return freeIds.Dequeue();
        }
        Logger.Instance.LogError(LogType.LogicFrame, "id不够分配");
        return -1;
    }
    //归还id
    void PushId(int id)
    {
        freeIds.Enqueue(id);
    }
    #endregion
}
