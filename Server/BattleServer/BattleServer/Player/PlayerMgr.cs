using BattleServer.Player;
using LeafNet;
using LeafNet.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



public class PlayerMgr : SingletonBase<PlayerMgr>
{
    Dictionary<string, ServerPlayer> allPlayers = new Dictionary<string, ServerPlayer>();
    Dictionary<Session, ServerPlayer> sessionPlayerDic = new Dictionary<Session, ServerPlayer>();



    public void Init()
    {
        //开启服务
        NetServiceMgr.Instance.AddService<LoginReqMsg>(OnLoginReq);
    }


    //玩家断开连接
    public void SetLogoutEvent(ServerEntity serverEntity)
    {
        serverEntity.clientDisconnectEvent += PlayerLogout;
    }

    private void PlayerLogout(Session obj)
    {
        ServerPlayer player = GetPlayer(obj);
        player.netState = NetState.Offline;
        NetLogger.GetInstance().Log("玩家注销:" + player.name);
    }

    /// <summary>
    /// 获取玩家类
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public ServerPlayer GetPlayer(string name)
    {
        if (allPlayers.ContainsKey(name))
        {
            return allPlayers[name];
        }
        return null;
    }

    public ServerPlayer GetPlayer(Session session)
    {
        if (sessionPlayerDic.ContainsKey(session))
        {
            return sessionPlayerDic[session];
        }
        return null;
    }

    //接收到登录请求

    void OnLoginReq(LoginReqMsg msg, Session session, NetworkEntity networkEntity)
    {
        LoginResMsg msgRes = new LoginResMsg();
        //有同名在线玩家
        if (allPlayers.ContainsKey(msg.name) && allPlayers[msg.name].netState == NetState.Online)
        {
            msgRes.state = LoginState.AnotherSameOnlinePlayer;
        }
        else
        {
            //新登录
            if (allPlayers.ContainsKey(msg.name) == false)
            {
                ServerPlayer player = new ServerPlayer();
                player.name = msg.name;
                player.playState = GameState.Normal;
                player.session = session;
                AddPlayer(player);
            }
            //断线重连
            else
            {
                ServerPlayer temp = allPlayers[msg.name];
                RemovePlayer(temp);
                temp.session = session;
                AddPlayer(temp);
            }

            allPlayers[msg.name].netState = NetState.Online;

            msgRes.name = allPlayers[msg.name].name;
            msgRes.state = LoginState.Success;
            msgRes.playState = allPlayers[msg.name].playState;
        }


        (networkEntity as ServerEntity).SendMessage(msgRes, session);
    }

    void AddPlayer(ServerPlayer player)
    {
        allPlayers.Add(player.name, player);
        sessionPlayerDic.Add(player.session, player);
    }

    void RemovePlayer(ServerPlayer player)
    {
        allPlayers.Remove(player.name);
        sessionPlayerDic.Remove(player.session);
    }
}

