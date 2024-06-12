using LeafNet;
using Messages.LogicFrame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messages.Messages
{
    [Serializable]
    public class FrameUpdateRes : MessageBase
    {
        public FrameCommand frameCommand;
    }
}
