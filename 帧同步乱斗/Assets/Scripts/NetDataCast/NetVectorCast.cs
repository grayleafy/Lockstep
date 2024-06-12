using BEPUutilities;
using Messages.NetVector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class NetVectorCast
{
    public static NetVector3 ToNetVector3(this UnityEngine.Vector3 vec)
    {
        NetVector3 result = new NetVector3();
        result.x = vec.x;
        result.y = vec.y;
        result.z = vec.z;
        return result;
    }
    public static BEPUutilities.Vector3 ToBEPUVector3(this NetVector3 vec)
    {
        BEPUutilities.Vector3 result = new BEPUutilities.Vector3();
        result.X = vec.x;
        result.Y = vec.y;
        result.Z = vec.z;
        return result;
    }
    public static UnityEngine.Vector3 ToVector3(this BEPUutilities.Vector3 vec)
    {
        UnityEngine.Vector3 result = new();
        result.x = (float)vec.X;
        result.y = (float)vec.Y;
        result.z = (float)vec.Z;
        return result;
    }
    public static BEPUutilities.Vector3 ToVector3(this UnityEngine.Vector3 vec)
    {
        BEPUutilities.Vector3 result = new(vec.x, vec.y, vec.z);
        return result;
    }

    public static UnityEngine.Quaternion ToUnityQuaternion(this BEPUutilities.Quaternion q)
    {
        UnityEngine.Quaternion quaternion = new UnityEngine.Quaternion((float)q.X, (float)q.Y, (float)q.Z, (float)q.W);
        return quaternion;
    }
    public static BEPUutilities.Quaternion ToBEPUQuaternion(this UnityEngine.Quaternion q)
    {
        BEPUutilities.Quaternion quaternion = new BEPUutilities.Quaternion(q.x, q.y, q.z, q.w);
        return quaternion;
    }
}
