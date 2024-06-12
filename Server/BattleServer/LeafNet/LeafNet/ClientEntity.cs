using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace LeafNet
{
    public class ClientEntity : NetworkEntity
    {
        private Session session;   //服务器通道
        public bool IsConnected
        {
            get
            {
                if (session == null || session.socket == null || session.socket.Connected == false)
                {
                    return false;
                }
                return true;
            }
        }
        public long lastSendTime;

        /// <summary>
        /// 开始连接，连接成功后在线程内执行回调
        /// </summary>
        /// <param name="callback"></param>
        public void BeginConnect(Action callback)
        {
            if (session == null)
            {
                session = new Session(new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp));
            }
            if (session.socket != null)
            {
                if (session.socket.Connected == true) session.socket.Shutdown(SocketShutdown.Both);
                session.socket.Close();
            }
            session.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            session.socket.NoDelay = true;
            session.socket.BeginConnect(NetSetting.GetInstance().serverIPEndPoint, asyncResult =>
            {
                try
                {
                    session.socket.EndConnect(asyncResult);
                    NetLogger.GetInstance().Log("连接成功");
                    callback.Invoke();
                }
                catch (SocketException ex)
                {
                    NetLogger.GetInstance().Log("连接失败" + ex.ToString());
                }
            }, null);
        }

        /// <summary>
        /// 发送消息给服务器
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public bool SendMessageToServer(MessageBase message)
        {
            if (session == null || session.socket == null || session.socket.Connected == false)
            {
                NetLogger.GetInstance().Log("发送失败， 未连接服务器");
                return false;
            }

            SendMessage(message, session);
            lastSendTime = TimeTool.GetInstance().GetTimeStamp();
            return true;
        }

        /// <summary>
        /// 接收网络数据
        /// </summary>
        /// <returns></returns>
        private bool Receive()
        {
            if (session == null || session.socket == null || session.socket.Connected == false)
            {
                NetLogger.GetInstance().Log("接收失败， 未连接服务器");
                return false;
            }

            if (session.socket.Poll(0, SelectMode.SelectRead))
            {
                session.Receive();
            }
            return true;
        }

        /// <summary>
        /// 序列化一条消息
        /// </summary>
        /// <returns></returns>
        private MessageBase PopMessage()
        {
            if (session == null || session.socket == null || session.socket.Connected == false)
            {
                NetLogger.GetInstance().Log("序列化消息失败， 未连接服务器");
                return null;
            }
            return session.PopMessage();
        }

        /// <summary>
        /// 单次帧更新
        /// </summary>
        public void FrameUpdate(int maxMessagesPerFrame)
        {
            if (IsConnected)
            {
                //收数据
                Receive();
                //处理消息
                for (int i = 0; i < maxMessagesPerFrame; i++)
                {
                    MessageBase msg = PopMessage();
                    if (msg != null)
                    {
                        msg.HandleProcess(session, this);
                    }
                    else
                    {
                        break;
                    }
                }
                ////发送心跳
                //long t = TimeTool.GetInstance().GetTimeStamp();
                ////if (t - lastSendTime > heartInterval)
                ////{
                ////    //SendMessageToServer(new HeartMsg());
                ////}
            }
        }
    }
}
