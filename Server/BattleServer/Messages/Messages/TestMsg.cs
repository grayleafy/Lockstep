using LeafNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace LeafNet
{
    [Serializable]
    public class TestMsg : MessageBase
    {
        public List<int> ids = new List<int>();
    }
}
