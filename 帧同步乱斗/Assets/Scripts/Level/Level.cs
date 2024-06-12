using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class Level
{
    public string name;
    public string description;
    public Sprite icon;
    public List<CharacterInfo> characters = new();
}
