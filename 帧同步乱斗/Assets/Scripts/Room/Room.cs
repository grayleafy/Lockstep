using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Room : IDisplayableModel
{
    Action<IModel> _modifyEvent;
    public Action<IModel> ModifiedEvent { get => _modifyEvent; set => _modifyEvent = value; }

    /// <summary>
    /// 房间名
    /// </summary>
    private string _name;
    public string Name
    {
        get
        {
            return _name;
        }
        set
        {
            _name = value;
            ModifiedEvent?.Invoke(this);
        }
    }
    private string _description;
    public string Description
    {
        get { return _description; }
        set
        {
            _description = value;
            ModifiedEvent?.Invoke(this);
        }
    }
    /// <summary>
    /// 地图名
    /// </summary>
    private int _levelId;
    public int LevelId
    {
        get
        {
            return _levelId;
        }
        set
        {
            _levelId = value;
            ModifiedEvent?.Invoke(this);
        }
    }

    public int MaxMember
    {
        get
        {
            return LevelMgr.Instance.GetLevel(LevelId).characters.Count;
        }
    }
    //成员
    private ModelList<RoomMember> _members = new();
    public ModelList<RoomMember> Members { get => _members; }
    //房主
    private string _owner;
    public string Owner { get => _owner; set { _owner = value; ModifiedEvent?.Invoke(this); } }
    //当前成员数
    public int CurrentMemberCount { get { return _members.Count; } }


    public Room()
    {

    }

    /// <summary>
    /// 重新赋值
    /// </summary>
    /// <param name="serverRoom"></param>
    public void FromServerRoom(ServerRoom serverRoom)
    {
        _name = serverRoom.name;
        _description = serverRoom.description;
        _levelId = serverRoom.levelId;
        _members.Reset(serverRoom.members, (serverRoomMember) =>
        {
            RoomMember member = new RoomMember();
            member.FromServerRoomMember(serverRoomMember);
            return member;
        });
        _owner = serverRoom.owner;
        ModifiedEvent?.Invoke(this);
    }

}
