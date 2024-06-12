using LeafNet.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace LeafNet
{
    public class NetSetting : SingletonBase<NetSetting>
    {
        public int port = 8888;                                 //端口
        public IPEndPoint serverIPEndPoint;                     //服务器地址
        public IPEndPoint localEndPoint;                        //本机地址
        public int clientsLimit = 1024;                         //服务器监听的最大客户端数量

        public NetSetting()
        {
            serverIPEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);
            localEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);
        }
    }
}
