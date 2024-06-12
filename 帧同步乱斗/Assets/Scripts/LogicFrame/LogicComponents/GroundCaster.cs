using BEPUphysics.BroadPhaseEntries;
using FixMath.NET;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCaster : LogicComponent
{
    /// <summary>
    /// 检测结果信息
    /// </summary>
    public BEPUphysics.RayCastResult hitInfo;

    //投射球体相关
    public BEPUphysics.CollisionShapes.ConvexShapes.SphereShape castSphere;
    BEPUutilities.RigidTransform startTransform;


    [Header("投射半径")]
    public Fix64 radius = 0.5f;
    [Header("投射距离")]
    public BEPUutilities.Vector3 castSweep = new BEPUutilities.Vector3(0, -1.2f, 0);
    [Header("起点偏移量")]
    public BEPUutilities.Vector3 originOffset = new BEPUutilities.Vector3(0, 1, 0);
    [Header("是否在地面上")]
    public bool isOnGround = false;

    private void Awake()
    {
        castSphere = new BEPUphysics.CollisionShapes.ConvexShapes.SphereShape(radius);
    }

    public override void OnLogicFrameUpdate(Fix64 dt)
    {
        base.OnLogicFrameUpdate(dt);
        RayCast();
    }

    //射线检测地面
    void RayCast()
    {
        //设置起点
        startTransform.Position = logicEntity.GetComponent<PhysEntity>().BEPUEntity.Position + logicEntity.GetComponent<PhysEntity>().GetWorldOffset() + originOffset;
        Func<BroadPhaseEntry, bool> filter = (broadPhaseEntry) =>
        {
            if ((int)broadPhaseEntry.Tag == LayerMask.NameToLayer("Ignore Raycast"))
            {
                return false;
            }
            return true;
        };
        if (PhysicsMgr.Instance.space.ConvexCast(castSphere, ref startTransform, ref castSweep, filter, out hitInfo))
        {
            isOnGround = true;
        }
        else
        {
            isOnGround = false;
        }
    }

    //bool Filter(BroadPhaseEntry broadPhaseEntry)
    //{
    //    GetComponent<PhysEntity>().BEPUEntity.Tag
    //}
}
