using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class StateEnd : MotionTrigger
{
    public override bool CheckCondition(MotionStateMachine stateMachine)
    {
        return stateMachine.currentStateEnd;
    }
}
