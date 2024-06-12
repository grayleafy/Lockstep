using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleServer.Battle
{
    /// <summary>
    /// 服务器战斗数据缓冲，用于维护用户的加载情况或者是一帧内的帧数据收发情况
    /// </summary>
    [Serializable]
    public class BattleCache
    {
        public List<bool> memberIsReady;

        public BattleCache(Battle battle)
        {
            memberIsReady = new List<bool>();
            for (int i = 0; i < battle.members.Count; i++)
            {
                memberIsReady.Add(false);
            }
        }
    }
}
