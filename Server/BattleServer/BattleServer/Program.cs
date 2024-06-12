using BattleServer.Battle;
using LeafNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleServer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ServerEntity serverEntity = new ServerEntity();


            //服务开启
            PlayerMgr.GetInstance().Init();
            PlayerMgr.GetInstance().SetLogoutEvent(serverEntity);
            RoomMgr.GetInstance().Init();
            BattleMgr.GetInstance().Init();

            for (int i = 0; i < 20; i++)
            {
                ServerRoom room = RoomMgr.GetInstance().InitRoom(i.ToString(), "服务器创建的测试空房间");
                RoomMgr.GetInstance().AddRoom(room);
            }

            serverEntity.StartServer();
        }
    }
}
