using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public static class GameObjectExtensions
{
    /// <summary>
    /// 在第一级子物体中去寻找对应名称的物体  
    /// </summary>
    /// <param name="self"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static GameObject FindInFirstChildren(this GameObject self, string name)
    {
        Transform transform = self.transform;
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            if (child.name == name)
            {
                return child.gameObject;
            }
        }
        return null;
    }

    /// <summary>
    /// 将物体以及子物体的layer都设置为layer
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="layer"></param>
    public static void SetLayerWithAllChildren(this GameObject gameObject, int layer)
    {
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            gameObject.transform.GetChild(i).gameObject.SetLayerWithAllChildren(layer);
        }

        gameObject.layer = layer;
    }
}
