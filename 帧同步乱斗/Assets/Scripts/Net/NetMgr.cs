using LeafNet;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetMgr : SingletonMono<NetMgr>
{
    //每一帧最大处理消息数
    public int maxMsgPerFrame = 20;
    //网络配置，可更改服务器ip地址等等
    public NetSetting Setting { get => NetSetting.GetInstance(); }
    //当前是否是连接状态
    public bool IsConnected { get => clientEntity.IsConnected; }

    ClientEntity clientEntity = new ClientEntity();
    bool lastFrameConnect = false;
    Action lostConnectEvent;    //当需要执行网络但是发现没有连接时调用



    public void BeginConnect(Action callback)
    {
        clientEntity.BeginConnect(callback);
    }

    /// <summary>
    /// 添加网络服务
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="service"></param>
    public void AddNetService<T>(Action<(MessageBase msg, Session senderSession, NetworkEntity networkEntity)> service) where T : MessageBase
    {
        NetEventCenter.GetInstance().AddEventListener(typeof(T).Name, service);
    }

    /// <summary>
    /// 移除网络服务
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="service"></param>
    public void RemoveNetService<T>(Action<(MessageBase msg, Session senderSession, NetworkEntity networkEntity)> service) where T : MessageBase
    {
        NetEventCenter.GetInstance().RemoveEventListener(typeof(T).Name, service);
    }


    /// <summary>
    /// 发送网络消息
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="msg"></param>

    public void SendMsg<T>(T msg) where T : MessageBase
    {
        if (clientEntity.IsConnected == false)
        {
            lostConnectEvent?.Invoke();
        }
        clientEntity.SendMessageToServer(msg);
    }

    /// <summary>
    /// 网络断开事件
    /// </summary>
    /// <param name="action"></param>
    public void AddLostConnectEvent(Action action)
    {
        lostConnectEvent += action;
    }
    /// <summary>
    /// 移除网络断开事件
    /// </summary>
    /// <param name="action"></param>
    public void RemoveLostConnectEvent(Action action)
    {
        lostConnectEvent -= action;
    }

    private void Update()
    {
        if (clientEntity.IsConnected)
        {
            clientEntity.FrameUpdate(maxMsgPerFrame);
        }

        if (lastFrameConnect == true && clientEntity.IsConnected == false)
        {
            lostConnectEvent?.Invoke();
        }

        lastFrameConnect = clientEntity.IsConnected;
    }
}
