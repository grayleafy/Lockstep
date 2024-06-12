using BEPUphysics;
using BEPUutilities;
using FixMath.NET;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsMgr : Singleton<PhysicsMgr>
{
    //物理更新步长时间
    public Fix64 tickStep = 1f / 15f;
    public BEPUphysics.Space space;

    public PhysicsMgr()
    {
        //关闭unity自带的物理引擎
        Physics.autoSimulation = false;
        //射线检测
        Physics.autoSyncTransforms = false;

        space = new BEPUphysics.Space();
        space.ForceUpdater.Gravity = new BEPUutilities.Vector3(0, -9.81f, 0);
        space.TimeStepSettings.TimeStepDuration = tickStep;
    }

    public void OnLogicUpdate()
    {
        space.Update();
    }

    public void AddPhysicsEntity(BEPUphysics.Entities.Entity phyEntity)
    {
        space.Add(phyEntity);
    }

    public void RemovePhysicsEntity(BEPUphysics.Entities.Entity phyEntity)
    {
        space.Remove(phyEntity);
    }
}
