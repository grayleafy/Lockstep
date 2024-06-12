using BEPUphysics.Entities;
using FixMath.NET;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]
public class PhysEntityCapsule : PhysEntity
{
    protected override Entity ConstructPhysEntity()
    {
        CapsuleCollider capsule = GetComponent<CapsuleCollider>();
        Vector3 axis = Vector3.zero;
        if (capsule.direction == 0)
        {
            axis = new Vector3(1, 0, 0);
        }
        else if (capsule.direction == 1)
        {
            axis = new Vector3(0, 1, 0);
        }
        else if (capsule.direction == 2)
        {
            axis = new Vector3(0, 0, 1);
        }
        BEPUutilities.Vector3 start = transform.TransformPoint(capsule.center - capsule.height / 2 * axis).ToVector3();
        BEPUutilities.Vector3 end = transform.TransformPoint(capsule.center + capsule.height / 2 * axis).ToVector3();
        BEPUphysics.Entities.Prefabs.Capsule entity = new BEPUphysics.Entities.Prefabs.Capsule(transform.position.ToVector3(), capsule.height, capsule.radius, mass);

        //材质
        BEPUphysics.Materials.Material material = new BEPUphysics.Materials.Material(0.2f, 0.2f, 0.2f);
        entity.Material = material;
        return entity;
    }
}
