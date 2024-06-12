using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messages.LogicFrame
{
    [Serializable]
    public class FrameCommand : IComparable<FrameCommand>
    {
        public int frameIndex;
        public List<EntityCommand> commands = new List<EntityCommand>();

        public FrameCommand(int frameIndex)
        {
            this.frameIndex = frameIndex;
        }

        public int CompareTo(FrameCommand other)
        {
            return frameIndex.CompareTo(other.frameIndex);
        }

        public static bool operator <(FrameCommand x, FrameCommand y)
        {
            return x.frameIndex < y.frameIndex;
        }

        public static bool operator >(FrameCommand x, FrameCommand y)
        {
            return x.frameIndex > y.frameIndex;
        }
    }
}
