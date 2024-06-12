using FixMath.NET;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LogicEntity))]
public class LogicComponent : MonoBehaviour
{
    public LogicEntity logicEntity;

    public virtual void Reset()
    {
        logicEntity = GetComponent<LogicEntity>();
        logicEntity.Reset();
    }



    public virtual void OnLogicFrameUpdate(Fix64 dt)
    {

    }
}
