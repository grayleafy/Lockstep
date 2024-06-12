using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnEnable()
    {
        Debug.Log("onenable");
    }

    private void OnDisable()
    {
        Debug.Log("disable");
    }
    private void OnDestroy()
    {
        Debug.Log("destroy");
    }
}
