using LeafNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messages
{
    [Serializable]
    public class RoomCreateReq : MessageBase
    {
        public string roomName;
        public string roomDescription;
    }
}
