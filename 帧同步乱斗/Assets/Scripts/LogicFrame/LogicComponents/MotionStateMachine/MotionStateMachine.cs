using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using FixMath.NET;

[Serializable]
[RequireComponent(typeof(CommandRecorder))]
public class MotionStateMachine : LogicComponent
{
    [Header("指令组件")]
    public CommandRecorder commandRecorder;
    [SerializeField]
    public InspectorBlackboard blackboard = new();

    [Header("所有状态")]
    public SerializableDictionaryFT<string, MotionState> states = new();
    [Header("状态转移条件，dic[当前状态名, [下一个状态名, 转移条件]]")]
    public SerializableDictionaryFF<string, SerializableDictionaryFT<string, MotionTrigger>> triggersDic = new();
    [Header("当前状态/默认状态")]
    public string currentState;

    //当前状态是否结束
    public bool currentStateEnd = false;



    public override void Reset()
    {
        base.Reset();
        commandRecorder = GetComponent<CommandRecorder>();
    }

    private void Start()
    {
        GetState(currentState).OnEnter(this);
    }
    private void Update()
    {
        GetState(currentState).OnRenderUpdate(this);
    }

    public override void OnLogicFrameUpdate(Fix64 dt)
    {
        base.OnLogicFrameUpdate(dt);
        FSMUpdate(dt);
    }

    /// <summary>
    /// 获取状态
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public MotionState GetState(string name)
    {
        if (states.ContainsKey(name) == false)
        {
            return null;
        }
        return states[name];
    }






    /// <summary>
    /// 状态机更新
    /// </summary>
    /// <param name="dt"></param>
    public void FSMUpdate(Fix64 dt)
    {
        var res = GetState(currentState).OnLogicUpdate(this, dt);
        if (res == MotionState.StateUpdateResult.End) { currentStateEnd = true; }
        string nextState = CheckTriggers(currentState);
        if (nextState != null)
        {
            TransitionToState(nextState);
        }
    }



    /// <summary>
    /// 转换状态
    /// </summary>
    /// <param name="nextState"></param>
    public void TransitionToState(string nextState)
    {
        GetState(currentState).OnExit(this);
        currentStateEnd = false;
        currentState = nextState;
        GetState(currentState).OnEnter(this);
    }

    //检测状态的所有触发器是否有满足的
    string CheckTriggers(string stateName)
    {
        var triggers = triggersDic[stateName];
        foreach (var pair in triggers)
        {
            if (pair.Value.CheckCondition(this) == true)
            {
                return pair.Key;
            }
        }
        return null;
    }
}
