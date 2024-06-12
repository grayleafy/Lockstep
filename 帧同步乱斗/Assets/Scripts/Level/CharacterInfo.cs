using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CharacterInfo : IModel
{
    public string fullName;
    public string description;
    //public GameObject prefab;

    private Action<IModel> _modifiedEvent;
    public Action<IModel> ModifiedEvent { get => _modifiedEvent; set => _modifiedEvent = value; }
}
