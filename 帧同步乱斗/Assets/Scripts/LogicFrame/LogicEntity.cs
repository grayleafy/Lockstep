using FixMath.NET;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 逻辑实体，执行顺序应该最先
/// </summary>
public class LogicEntity : MonoBehaviour
{
    public int id;
    public Fix64 logicTimeScale = 1;

    [Header("逻辑组件")]
    public List<LogicComponent> logicComponents = new();

    /// <summary>
    /// 重新更新逻辑组件列表
    /// </summary>
    public void Reset()
    {
        logicComponents = new List<LogicComponent>(GetComponents<LogicComponent>());
    }

    private void Awake()
    {
        LogicFrameMgr.Instance.LogicEntityLogin(this);
    }

    private void OnDestroy()
    {
        LogicFrameMgr.Instance.LogicEntityLogout(this);
    }

    #region 生命周期
    public void OnLogicFrameUpdate(Fix64 dt)
    {
        foreach (var component in logicComponents)
        {
            component.OnLogicFrameUpdate(dt);
        }
    }
    #endregion

    #region 组件函数
    public T AddLogicComponent<T>() where T : LogicComponent
    {
        throw new System.NotImplementedException();
    }
    #endregion
}
