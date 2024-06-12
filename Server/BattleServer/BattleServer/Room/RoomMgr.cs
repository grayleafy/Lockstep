using BattleServer.Player;
using LeafNet;
using LeafNet.Base;
using Messages;
using Messages.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

public class RoomMgr : SingletonBase<RoomMgr>
{
    Dictionary<string, ServerRoom> allRooms = new Dictionary<string, ServerRoom>();

    public void Init()
    {
        NetServiceMgr.Instance.AddService<RoomCreateReq>(OnRoomCreateReq);
        NetServiceMgr.Instance.AddService<JoinRoomReq>(OnJoinRoomReq);
        NetServiceMgr.Instance.AddService<SearchRoomsReq>(OnSearchRooms);
        NetServiceMgr.Instance.AddService<RoomMemberCharacterChangeReq>(OnRoomMemberCharacterChange);


    }
    #region 网络服务
    //创建房间服务
    void OnRoomCreateReq(RoomCreateReq msg, Session session, NetworkEntity networkEntity)
    {
        RoomCreateRes res = new RoomCreateRes();
        if (allRooms.ContainsKey(msg.roomName))
        {
            res.state = RoomCreateState.ExsitSameNameRoom;
            networkEntity.SendMessage(res, session);
            return;
        }

        ServerRoom room = InitRoom(msg.roomName, msg.roomDescription);
        room.owner = PlayerMgr.GetInstance().GetPlayer(session).name;
        AddRoom(room);

        res.state = RoomCreateState.Succces;
        res.room = room;
        networkEntity.SendMessage(res, session);
    }
    //加入房间服务
    void OnJoinRoomReq(JoinRoomReq req, Session session, NetworkEntity networkEntity)
    {
        JoinRoomRes res = new JoinRoomRes();
        res.state = JoinRoom(PlayerMgr.GetInstance().GetPlayer(session).name, req.roomName);



        //成功
        if (res.state == JoinRoomState.Success)
        {
            //回复
            res.room = GetRoom(req.roomName);
            networkEntity.SendMessage(res, session);

            //向房间内的其它成员发送更新
            BroadcastRoomModifyMsg(req.roomName, session, networkEntity);
        }
    }
    //查找获取房间服务
    void OnSearchRooms(SearchRoomsReq req, Session session, NetworkEntity networkEntity)
    {
        List<ServerRoom> rooms = new List<ServerRoom>();
        foreach (ServerRoom room in allRooms.Values)
        {
            if (req.searchWords == "" || room.name.Contains(req.searchWords))
            {
                rooms.Add(room);
            }
        }

        SearchRoomsRes res = new SearchRoomsRes();
        res.rooms = rooms;
        networkEntity.SendMessage(res, session);
    }
    //成员改变选择的角色服务
    void OnRoomMemberCharacterChange(RoomMemberCharacterChangeReq req, Session session, NetworkEntity networkEntity)
    {
        //玩家
        ServerPlayer player = PlayerMgr.GetInstance().GetPlayer(session);
        //先找到房间
        ServerRoom room = player.room;
        if (room == null)
        {
            NetLogger.GetInstance().Log("改变选择角色失败， 未找到玩家的房间");
            return;
        }

        //修改
        for (int i = 0; i < room.members.Count; i++)
        {
            if (room.members[i].name == player.name)
            {
                room.members[i].characterId = req.newCharacterId;
                break;
            }
        }
        //广播
        BroadcastRoomModifyMsg(room.name, null, networkEntity);
    }


    /// <summary>
    /// 在房间内广播协议
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="room"></param>
    /// <param name="ignore"></param>
    /// <param name="networkEntity"></param>
    public void Broadcast(MessageBase msg, ServerRoom room, Session ignore, NetworkEntity networkEntity)
    {
        for (int i = 0; i < room.members.Count; i++)
        {
            if (ignore == null || room.members[i].name != PlayerMgr.GetInstance().GetPlayer(ignore).name)
            {
                networkEntity.SendMessage(msg, PlayerMgr.GetInstance().GetPlayer(room.members[i].name).session);
            }
        }
    }
    #endregion

    /// <summary>
    /// 获取房间
    /// </summary>
    /// <param name="roonName"></param>
    /// <returns></returns>
    public ServerRoom GetRoom(string roonName)
    {
        if (allRooms.ContainsKey(roonName))
        {
            return allRooms[roonName];
        }
        return null;
    }

    //新建一个房间并初始化
    public ServerRoom InitRoom(string roomName, string description)
    {
        ServerRoom room = new ServerRoom();
        room.name = roomName;
        room.description = description;
        return room;
    }

    //添加一个房间
    public bool AddRoom(ServerRoom serverRoom)
    {
        if (!allRooms.ContainsKey(serverRoom.name))
        {
            allRooms.Add(serverRoom.name, serverRoom);
            return true;
        }
        return false;
    }




    //玩家加入房间
    JoinRoomState JoinRoom(string player, string roomName)
    {
        ServerRoom room = GetRoom(roomName);
        if (room == null)
        {
            return JoinRoomState.NotExistRoom;
        }

        if (room.Join(player))
        {
            PlayerMgr.GetInstance().GetPlayer(player).room = room;
            return JoinRoomState.Success;
        }

        return JoinRoomState.RoomIsFull;
    }

    //广播房间修改协议
    void BroadcastRoomModifyMsg(string roomName, Session ignore, NetworkEntity networkEntity)
    {
        //向房间内的其它成员发送更新
        ServerRoom room = GetRoom(roomName);
        RoomModify roomModify = new RoomModify();
        roomModify.room = room;
        Broadcast(roomModify, room, ignore, networkEntity);
    }
}

