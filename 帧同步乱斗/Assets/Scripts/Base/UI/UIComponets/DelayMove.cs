using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 延迟和偏移组件
/// </summary>
public class DelayMove : UIEffect
{
    public float deltaTime = 0.2f;
    public List<Transform> children;

    public List<Vector3> originPos;

    public void Reset()
    {
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            children.Add((gameObject.transform as RectTransform).GetChild(i));
        }

        originPos = new List<Vector3>();
        for (int i = 0; i < children.Count; i++)
        {
            originPos.Add(children[i].localPosition);
        }
    }

    public void ResetChildren()
    {
        for (int i = 0; i < children.Count; i++)
        {
            children[i].localPosition = originPos[i];
        }
    }


    public void SetOffset(Vector3 offset)
    {
        for (int i = 0; i < children.Count; i++)
        {
            children[i].position = children[i].position + offset;
        }
    }

    public void Move(Vector3 dir, float duration, bool reverse)
    {
        if (reverse == true)
        {
            for (int i = 0; i < children.Count; i++)
            {
                children[i].DOMove(children[i].transform.position + dir, duration).SetDelay(deltaTime * i);
            }
        }
        else
        {
            for (int i = 0; i < children.Count; i++)
            {
                children[children.Count - 1 - i].DOMove(children[children.Count - 1 - i].transform.position + dir, duration).SetDelay(deltaTime * i);
            }
        }
    }

    /// <summary>
    /// 从偏移的地方移动回来
    /// </summary>
    /// <param name="offset"></param>
    /// <param name="duration"></param>
    /// <param name="reverse"></param>
    public void MoveFromOffset(Vector3 offset, float duration, bool reverse)
    {
        SetOffset(offset);
        Move(-offset, duration, reverse);
    }
}
