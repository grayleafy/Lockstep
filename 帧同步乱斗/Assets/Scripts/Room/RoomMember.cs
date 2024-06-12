using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RoomMember : IDisplayableModel
{
    private Action<IModel> _modifyEvent;
    public Action<IModel> ModifiedEvent { get => _modifyEvent; set => _modifyEvent = value; }

    private string _name;
    public string Name { get => _name; set { _name = value; ModifiedEvent?.Invoke(this); } }
    private int _characterId = 0;
    public int CharacterId { get => _characterId; set { _characterId = value; ModifiedEvent?.Invoke(this); } }

    public void FromServerRoomMember(Messages.SharedData.RoomPlayer roomPlayer)
    {
        _name = roomPlayer.name;
        _characterId = roomPlayer.characterId;
        ModifiedEvent?.Invoke(this);
    }
}
