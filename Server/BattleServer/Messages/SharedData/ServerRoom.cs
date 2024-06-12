using Messages.SharedData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[Serializable]
public class ServerRoom
{
    public string name;
    public string description;
    public int levelId = 0;
    public List<RoomPlayer> members = new List<RoomPlayer>();
    public string owner;

    /// <summary>
    /// 加入成员，容量判断放在客户端本地
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    public bool Join(string player)
    {
        RoomPlayer roomPlayer = new RoomPlayer();
        roomPlayer.name = player;
        members.Add(roomPlayer);
        return true;
    }
}

