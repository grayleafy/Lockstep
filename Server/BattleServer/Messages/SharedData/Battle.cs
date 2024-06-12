using Messages.LogicFrame;
using Messages.SharedData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleServer.Battle
{
    [Serializable]
    public class Battle
    {
        //房间信息
        public string roomName;
        public int levelId;
        public List<RoomPlayer> members = new List<RoomPlayer>();
        public string owner;

        //帧数据
        public SortedSet<FrameCommand> frameCommands = new SortedSet<FrameCommand>();

        //其它，随机种子等

        /// <summary>
        /// 添加下一帧，操作置为空
        /// </summary>
        public void AddNextEmptyFrame()
        {
            int nextIndex = frameCommands.Count;
            frameCommands.Add(new FrameCommand(nextIndex));
        }

        public Battle CopyInfo()
        {
            Battle battleInfo = new Battle();
            battleInfo.owner = owner;
            battleInfo.members = members;
            battleInfo.roomName = roomName;
            battleInfo.levelId = levelId;
            return battleInfo;
        }
    }
}
