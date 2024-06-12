using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    private void FixedUpdate()
    {
        LogicFrameMgr.Instance.LogicFrameUpdate(PhysicsMgr.Instance.tickStep);
    }


}
