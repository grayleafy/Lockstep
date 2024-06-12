using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class MotionTrigger
{
    /// <summary>
    /// 每一帧Update中检测
    /// </summary>
    /// <param name="stateMachine"></param>
    /// <returns></returns>
    public abstract bool CheckCondition(MotionStateMachine stateMachine);
}
