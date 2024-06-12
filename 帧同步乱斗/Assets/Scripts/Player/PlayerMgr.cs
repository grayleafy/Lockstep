using LeafNet;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;



public class PlayerMgr : Singleton<PlayerMgr>
{
    public HostPlayer hostPlayer = new();

    public Action<LoginResMsg> loginEndAction;

    public PlayerMgr()
    {
        NetServiceMgr.Instance.AddService<LoginResMsg>(LoginEnd);
    }

    public void BeginLogin(string name, Action<LoginResMsg> callback)
    {
        loginEndAction = callback;

        //如果没连接则连接
        if (NetMgr.Instance.IsConnected == false)
        {
            NetMgr.Instance.BeginConnect(() => NetMgr.Instance.SendMsg<LoginReqMsg>(new LoginReqMsg() { name = name }));
        }
        else
        {
            //发送协议
            NetMgr.Instance.SendMsg<LoginReqMsg>(new LoginReqMsg() { name = name });
        }

    }

    public void LoginEnd(LoginResMsg msg, Session session, NetworkEntity networkEntity)
    {
        if (msg.state == LoginState.Success)
        {
            hostPlayer.name = msg.name;
            if (msg.playState == GameState.Normal)
            {

            }
            //断线重连
            else if (msg.playState == GameState.InGaming)
            {
                UIMgr.Instance.ShowPanel<TooltipPanel>("TooltipPanel", (panel) =>
                {
                    panel.SetTooltip("重新连接", "玩家正在游戏中，请重新连接");
                    panel.okButton.onClick.AddListener(BattleMgr.Instance.BeginReConnect);
                });
            }
        }

        Logger.Instance.Log(LogType.NetService, msg.state + ", " + msg.playState);
        loginEndAction?.Invoke(msg);
    }
}
