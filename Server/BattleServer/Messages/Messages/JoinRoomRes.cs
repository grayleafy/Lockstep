using LeafNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messages.Messages
{
    public enum JoinRoomState
    {
        Success,
        RoomIsFull,
        NotExistRoom,
    }
    [Serializable]
    public class JoinRoomRes : MessageBase
    {
        public JoinRoomState state;
        public ServerRoom room;
    }
}
