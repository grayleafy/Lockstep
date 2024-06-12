using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace LeafNet
{
    public enum ReceiveStatus
    {
        Success,           //成功
        Disconnected,      //断开连接
        BufferOverflow,    //缓冲区不够
        SockerException    //socket异常
    }


    public class Session
    {
        public Socket socket;           //socket
        public ByteArray receiveCache;   //接收缓冲区
        public long lastPingTime;       //最近的ping时间，服务器会用到

        public Session(Socket socket)
        {
            this.socket = socket;
            receiveCache = new ByteArray();
            UpdatePingTime();
        }

        /// <summary>
        /// 更新最近Ping时间
        /// </summary>
        public void UpdatePingTime()
        {
            lastPingTime = TimeTool.GetInstance().GetTimeStamp();
        }

        /// <summary>
        /// 查询网络缓冲区的消息并接收
        /// </summary>
        public ReceiveStatus Receive()
        {
            //NetLogger.GetInstance().Log("网络缓冲区接收到消息，来自" + socket.RemoteEndPoint.ToString());

            //缓存区不够
            if (receiveCache.remain <= 0)
            {
                receiveCache.MoveBytes();
                if (receiveCache.remain <= 0)
                {
                    receiveCache.ReSize(receiveCache.length * 2);
                }
                if (receiveCache.remain <= 0)
                {
                    NetLogger.GetInstance().Log("接收失败，缓冲区不足");
                    return ReceiveStatus.BufferOverflow;
                }
            }

            try
            {
                int count = socket.Receive(receiveCache.bytes, receiveCache.writeIdx, receiveCache.remain, 0);
                receiveCache.writeIdx += count;
                receiveCache.CheckAndMoveBytes();
                if (count == 0)
                {
                    NetLogger.GetInstance().Log("接收消息长度为0， 断开连接");
                    return ReceiveStatus.Disconnected;
                }
            }
            catch (SocketException ex)
            {
                NetLogger.GetInstance().Log("socket异常" + ex.ToString());
                return ReceiveStatus.SockerException;
            }

            UpdatePingTime();
            return ReceiveStatus.Success;
        }

        /// <summary>
        /// 序列化并取出一条消息,若长度不够则返回null
        /// </summary>
        /// <returns></returns>
        public MessageBase PopMessage()
        {
            return MessageBase.DeserializeFromByteArray(receiveCache);
        }
    }
}
