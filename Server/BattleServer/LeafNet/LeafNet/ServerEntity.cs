using LeafNet.LeafNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;


namespace LeafNet
{
    public class ServerEntity : NetworkEntity
    {
        Socket listenfd;                                                                             //监听socket
        long heartTimeoutInterval = 5000;                                                            //心跳超时则断开连接
        List<Socket> checkRead = new List<Socket>();                                                 //select检测的socket列表
        Dictionary<Socket, Session> sessionDic = new Dictionary<Socket, Session>();                  //已连接的会话



        /// <summary>
        /// 新连接建立后触发的事件，用于处理业务层逻辑，不涉及网络层
        /// </summary>
        public event Action<Session> clientConnectEvent;
        /// <summary>
        /// 连接断开前触发的事件，用于处理业务层逻辑，不涉及网络层
        /// </summary>
        public event Action<Session> clientDisconnectEvent;


        /// <summary>
        /// 服务器启动
        /// </summary>
        public void StartServer()
        {
            NetLogger.GetInstance().Log("服务器启动，端口号" + NetSetting.GetInstance().port);
            listenfd = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp); //监听socket
            listenfd.NoDelay = true;
            listenfd.Bind(NetSetting.GetInstance().localEndPoint);
            listenfd.Listen(NetSetting.GetInstance().clientsLimit);


            checkRead = new List<Socket>();
            while (true)
            {
                //定时器
                ServerTimer.GetInstance().Tick(this);

                ResetCheckSockets();
                Socket.Select(checkRead, null, null, 10);
                for (int i = checkRead.Count - 1; i >= 0; --i)
                {
                    if (checkRead[i] == listenfd)
                    {
                        ReadListenfd(listenfd);
                    }
                    else
                    {
                        ReadClientfd(checkRead[i]);
                    }
                }


            }
        }


        //重置select列表,并且根据心跳断开连接
        void ResetCheckSockets()
        {
            checkRead.Clear();
            checkRead.Add(listenfd);

            long currentTime = TimeTool.GetInstance().GetTimeStamp();
            List<Socket> sockets = sessionDic.Keys.ToList();
            foreach (var socket in sockets)
            {
                //if ((currentTime - sessionDic[socket].lastPingTime) <= heartTimeoutInterval)
                //{
                checkRead.Add(socket);
                //}
                //else
                //{
                //    //NetLogger.GetInstance().Log("心跳超时，断开连接" + socket.RemoteEndPoint.ToString());
                //    //CloseSession(sessionDic[socket]);
                //}
            }
        }

        //建立新连接会话
        void ReadListenfd(Socket listenfd)
        {
            try
            {
                Socket clientfd = listenfd.Accept();
                clientfd.NoDelay = true;
                NetLogger.GetInstance().Log("接收连接" + clientfd.RemoteEndPoint.ToString());
                Session session = new Session(clientfd);
                sessionDic.Add(clientfd, session);
                if (clientConnectEvent != null) clientConnectEvent.Invoke(session);
            }
            catch (SocketException ex)
            {
                NetLogger.GetInstance().Log("接收连接失败" + ex.ToString());
            }
        }

        //会话接收到字节
        private void ReadClientfd(Socket socket)
        {
            if (sessionDic.ContainsKey(socket) == false)
            {
                NetLogger.GetInstance().Log("接收到了不在会话字典中的客户端发来的消息" + socket.RemoteEndPoint.ToString());
            }
            var session = sessionDic[socket];

            //接收数据
            var status = session.Receive();
            //接收失败则断开连接
            if (status != ReceiveStatus.Success)
            {
                CloseSession(session);
            }



            //如果能序列化出一条消息，则处理
            MessageBase msg;
            while ((msg = session.PopMessage()) != null)
            {
                msg.HandleProcess(session, this);
            }
        }

        /// <summary>
        /// 断开连接
        /// </summary>
        /// <param name="session"></param>
        public void CloseSession(Session session)
        {
            if (clientDisconnectEvent != null) clientDisconnectEvent.Invoke(session);
            sessionDic.Remove(session.socket);
            session.socket.Shutdown(SocketShutdown.Both);
            session.socket.Close();
        }
    }
}
