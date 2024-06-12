using LeafNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class MsgForwarder
{

}

public class MsgForwarder<T> : MsgForwarder where T : MessageBase
{
    public Action<T, Session, NetworkEntity> action;

    public void HandleMsg((MessageBase msg, Session senderSession, NetworkEntity networkEntity) args)
    {
        NetLogger.GetInstance().Log(typeof(T).Name + " ");
        action.Invoke(args.msg as T, args.senderSession, args.networkEntity);
    }
}

public class NetServiceMgr
{
    private static NetServiceMgr _instance = new NetServiceMgr();
    public static NetServiceMgr Instance { get { return _instance; } }

    private Dictionary<string, MsgForwarder> msgForwards = new Dictionary<string, MsgForwarder>();


    /// <summary>
    /// 添加服务
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="action"></param>
    public void AddService<T>(Action<T, Session, NetworkEntity> action) where T : MessageBase
    {
        MsgForwarder<T> msgForward = new MsgForwarder<T>() { action = action };
        msgForwards.Add(typeof(T).Name, msgForward);
        NetEventCenter.GetInstance().AddEventListener<(MessageBase msg, Session senderSession, NetworkEntity networkEntity)>(typeof(T).Name, msgForward.HandleMsg);
    }

    /// <summary>
    /// 移除服务
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="action"></param>
    public void RemoveService<T>(Action<T, Session, NetworkEntity> action) where T : MessageBase
    {
        if (msgForwards.ContainsKey(typeof(T).Name) == false)
        {
            return;
        }
        MsgForwarder<T> msgForward = msgForwards[typeof(T).Name] as MsgForwarder<T>;
        NetEventCenter.GetInstance().RemoveEventListener<(MessageBase msg, Session senderSession, NetworkEntity networkEntity)>(typeof(T).Name, msgForward.HandleMsg);
    }

}

