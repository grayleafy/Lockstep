using LeafNet;
using Messages;
using Messages.Messages;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RoomMgr : Singleton<RoomMgr>
{
    public ModelList<Room> allRooms = new ModelList<Room>();
    public Room currentRoom = new();


    public RoomMgr()
    {
        NetServiceMgr.Instance.AddService<RoomCreateRes>(OnRoomCreateRes);
        NetServiceMgr.Instance.AddService<JoinRoomRes>(OnJoinRoomRes);
        NetServiceMgr.Instance.AddService<RoomModify>(OnRoomModify);
        NetServiceMgr.Instance.AddService<SearchRoomsRes>(OnSearchRoomRes);
    }

    #region 网络服务
    public void OnRoomCreateRes(RoomCreateRes res, Session session, NetworkEntity networkEntity)
    {
        Logger.Instance.Log(LogType.NetService, res);

        if (res.state == RoomCreateState.Succces)
        {
            currentRoom.FromServerRoom(res.room);
            EventCenter.Instance.EventTrigger(EventName.CreateRoomSuccess);

            //房主加入房间
            JoinRoomBegin(currentRoom.Name);
        }
    }
    void OnJoinRoomRes(JoinRoomRes res, Session session, NetworkEntity networkEntity)
    {
        if (res.state == JoinRoomState.Success)
        {
            currentRoom.FromServerRoom(res.room);
        }

        EventCenter.Instance.EventTrigger<JoinRoomState>(EventName.OnJoinRoom, res.state);
    }
    void OnRoomModify(RoomModify msg, Session session, NetworkEntity networkEntity)
    {
        //房间中包含自己
        bool roomContain = false;
        for (int i = 0; i < msg.room.members.Count; i++)
        {
            if (msg.room.members[i].name == PlayerMgr.Instance.hostPlayer.name)
            {
                roomContain = true;
                break;
            }
        }

        if (roomContain)
        {
            currentRoom.FromServerRoom(msg.room);
            EventCenter.Instance.EventTrigger(EventName.OnRoomModifed);
        }
    }

    void OnSearchRoomRes(SearchRoomsRes res, Session session, NetworkEntity networkEntity)
    {
        allRooms.Reset(res.rooms, (serverRoom) =>
        {
            Room room = new Room();
            room.FromServerRoom(serverRoom);
            return room;
        });
    }
    #endregion

    /// <summary>
    /// 开始创建房间
    /// </summary>
    /// <param name="roomName"></param>
    /// <param name="description"></param>
    public void CreateRoomBegin(string roomName, string description)
    {
        //level
        RoomCreateReq req = new RoomCreateReq();
        req.roomName = roomName;
        req.roomDescription = description;


        NetMgr.Instance.SendMsg(req);
    }





    public void JoinRoomBegin(string roomName)
    {
        JoinRoomReq req = new JoinRoomReq();
        req.roomName = roomName;
        NetMgr.Instance.SendMsg(req);
    }

    /// <summary>
    /// 搜索房间
    /// </summary>
    /// <param name="searchWord"></param>
    public void SeachRoomsBegin(string searchWord)
    {
        SearchRoomsReq req = new SearchRoomsReq();
        req.searchWords = searchWord;
        NetMgr.Instance.SendMsg(req);
    }

    /// <summary>
    /// 改变选择的角色
    /// </summary>
    /// <param name="index"></param>
    public void ChangeCharacter(int index)
    {
        NetMgr.Instance.SendMsg(new RoomMemberCharacterChangeReq() { newCharacterId = index });
    }
}
