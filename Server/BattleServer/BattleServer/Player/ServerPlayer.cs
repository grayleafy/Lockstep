using LeafNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleServer.Player
{
    public enum NetState
    {
        Online,
        Offline,
    }

    public class ServerPlayer
    {
        public string name;
        public NetState netState;
        public GameState playState;
        public Session session;
        public ServerRoom room;
    }
}
