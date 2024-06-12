using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public static class UIUtility
{

    public static void SetChildrenAlpha(Transform root, float alpha)
    {
        var images = root.GetComponentsInChildren<Image>();
        foreach (var image in images)
        {
            var color = image.color;
            color.a = alpha;
            image.color = color;
        }

        var texts = root.GetComponentsInChildren<TextMeshProUGUI>();
        foreach (var text in texts)
        {
            text.alpha = alpha;
        }


    }

    /// <summary>
    /// 矩形宽度
    /// </summary>
    /// <param name="rect"></param>
    /// <returns></returns>
    public static float Width(this RectTransform rect)
    {
        Vector3[] corner = new Vector3[4];
        rect.GetWorldCorners(corner);
        return corner[2].x - corner[0].x;
    }

    /// <summary>
    /// 矩形高度
    /// </summary>
    /// <param name="rect"></param>
    /// <returns></returns>
    public static float Height(this RectTransform rect)
    {
        Vector3[] corner = new Vector3[4];
        rect.GetWorldCorners(corner);
        return corner[2].y - corner[0].y;
    }

    /// <summary>
    /// 设置矩阵大小，指定一个不变的角
    /// </summary>
    /// <param name="rect"></param>
    /// <param name="size"></param>
    /// <param name="startCorner"></param>
    public static void SetSize(this RectTransform rect, Vector2 size, GridLayoutGroup.Corner startCorner)
    {
        float deltaX = size.x - rect.Width();
        float deltaY = size.y - rect.Height();

        if (startCorner == GridLayoutGroup.Corner.LowerLeft)
        {
            rect.offsetMax = new Vector2(rect.offsetMax.x + deltaX, rect.offsetMax.y + deltaY);
        }
        else if (startCorner == GridLayoutGroup.Corner.LowerRight)
        {
            rect.offsetMax = new Vector2(rect.offsetMax.x, rect.offsetMax.y + deltaY);
            rect.offsetMin = new Vector2(rect.offsetMin.x - deltaX, rect.offsetMin.y);
        }
        else if (startCorner == GridLayoutGroup.Corner.UpperLeft)
        {
            rect.offsetMax = new Vector2(rect.offsetMax.x + deltaX, rect.offsetMax.y);
            rect.offsetMin = new Vector2(rect.offsetMin.x, rect.offsetMin.y - deltaY);
        }
        else if (startCorner == GridLayoutGroup.Corner.UpperRight)
        {
            rect.offsetMin = new Vector2(rect.offsetMin.x - deltaX, rect.offsetMin.y - deltaY);
        }
    }

    public static void SetCornerPos(this RectTransform rect, Vector3 pos, GridLayoutGroup.Corner startCorner)
    {
        Vector3[] corners = new Vector3[4];
        rect.GetWorldCorners(corners);
        Vector3 delta = Vector3.zero;
        if (startCorner == GridLayoutGroup.Corner.LowerLeft)
        {
            delta = rect.position - corners[0];
        }
        else if (startCorner == GridLayoutGroup.Corner.UpperLeft)
        {
            delta = rect.position - corners[1];
        }
        else if (startCorner == GridLayoutGroup.Corner.UpperRight)
        {
            delta = rect.position - corners[2];
        }
        else if (startCorner == GridLayoutGroup.Corner.LowerRight)
        {
            delta = rect.position - corners[3];
        }

        rect.position = pos + delta;
    }
}
