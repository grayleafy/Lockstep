using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelConfig", menuName = "配置/关卡配置", order = 0)]
public class LevelConfig : ScriptableObject
{
    public List<Level> levels = new List<Level>();
}
