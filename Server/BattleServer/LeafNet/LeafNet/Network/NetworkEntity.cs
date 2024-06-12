using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace LeafNet
{
    public class NetworkEntity
    {
        /// <summary>
        /// 发送消息,异步实现
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="session"></param>
        public void SendMessage(MessageBase msg, Session session)
        {
            Socket socket = session.socket;
            if (socket.Connected == false)
            {
                NetLogger.GetInstance().Log("连接已断开，发送消息失败" + socket.ToString());
                return;
            }

            byte[] bytes = msg.SerializeWithHead();
            try
            {
                socket.BeginSend(bytes, 0, bytes.Length, SocketFlags.None, (asyncResult) =>
                {
                    try
                    {
                        //完成消息发送
                        int len = socket.EndSend(asyncResult);
                    }
                    catch (Exception ex)
                    {
                        NetLogger.GetInstance().Log("发送失败" + ex.Message + " " + ex.StackTrace);
                    }
                }, null);
            }
            catch (SocketException ex)
            {
                NetLogger.GetInstance().Log("发送失败" + ex.Message + " " + ex.StackTrace);
            }
        }
    }
}
