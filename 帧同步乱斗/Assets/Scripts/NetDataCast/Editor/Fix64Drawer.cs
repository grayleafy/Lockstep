using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using FixMath.NET;

[CustomPropertyDrawer(typeof(Fix64))]
public class Fix64Drawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        //绘制标签
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        long x = property.FindPropertyRelative("RawValue").longValue;
        Fix64 temp = new Fix64();
        temp.RawValue = x;
        float f = (float)temp;
        f = EditorGUI.FloatField(position, f);
        temp = f;
        property.FindPropertyRelative("RawValue").longValue = temp.RawValue;
        EditorGUI.EndProperty();
    }
}
