using LeafNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messages
{

    public enum RoomCreateState
    {
        Succces,
        ExsitSameNameRoom,
    }


    [Serializable]
    public class RoomCreateRes : MessageBase
    {
        public RoomCreateState state;
        public ServerRoom room;
    }
}
