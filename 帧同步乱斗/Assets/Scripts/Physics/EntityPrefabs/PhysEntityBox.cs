using BEPUphysics.Entities;
using BEPUutilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class PhysEntityBox : PhysEntity
{
    protected override Entity ConstructPhysEntity()
    {
        BoxCollider boxCollider = GetComponent<BoxCollider>();
        BEPUphysics.Entities.Prefabs.Box box = new BEPUphysics.Entities.Prefabs.Box(transform.TransformPoint(boxCollider.center).ToVector3(), boxCollider.size.x * transform.lossyScale.x, boxCollider.size.y * transform.lossyScale.y, boxCollider.size.z * transform.lossyScale.z);
        offset = transform.InverseTransformDirection(-boxCollider.center).ToVector3();
        return box;
    }
}
