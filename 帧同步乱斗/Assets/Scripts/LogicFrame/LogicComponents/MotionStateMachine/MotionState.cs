using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using FixMath.NET;

[Serializable]
public abstract class MotionState
{
    public enum StateUpdateResult
    {
        Running,
        End,
    }

    public virtual void OnEnter(MotionStateMachine stateMachine)
    {

    }

    public virtual void OnExit(MotionStateMachine stateMachine)
    {

    }

    /// <summary>
    /// 更新，返回是否运行结束
    /// </summary>
    /// <param name="stateMachine"></param>
    /// <param name="dt"></param>
    /// <returns></returns>
    public abstract StateUpdateResult OnLogicUpdate(MotionStateMachine stateMachine, Fix64 dt);
    /// <summary>
    /// 渲染层更新
    /// </summary>
    /// <param name="stateMachine"></param>
    public virtual void OnRenderUpdate(MotionStateMachine stateMachine) { }
}
