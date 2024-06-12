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
    public class ReconnectLevelLoadFinishRes : MessageBase
    {
        /// <summary>
        /// 是否发送完毕
        /// </summary>
        public bool msgEnd;
        public List<FrameCommand> frameCommands;
    }
}
