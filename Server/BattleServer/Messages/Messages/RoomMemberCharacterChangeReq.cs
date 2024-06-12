using LeafNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messages.Messages
{
    [Serializable]
    public class RoomMemberCharacterChangeReq : MessageBase
    {
        public int newCharacterId;
    }
}
