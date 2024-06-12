using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HighLightRoomPanel : BasePanel, IView
{
    public Image icon;
    public TextMeshProUGUI roomName;
    public TextMeshProUGUI roomDescription;
    public TextMeshProUGUI memberCount;
    public TextMeshProUGUI levelName;

    private Room data;
    IModel IView.Data { get => data; set => data = value as Room; }

    public void RefreshView(IModel data)
    {
        if (data == null)
        {
            SetEmpty();
            return;
        }


        Room room = data as Room;
        if (room == null)
        {
            Logger.Instance.LogError(LogType.ClassCast, "model to room");
            return;
        }

        roomName.text = room.Name;
        roomDescription.text = "\u3000\u3000" + room.Description;
        memberCount.text = room.CurrentMemberCount.ToString() + "<size=32>/" + room.MaxMember.ToString();
        levelName.text = LevelMgr.Instance.GetLevel(room.LevelId).name;
    }

    //空
    public void SetEmpty()
    {
        roomName.text = "请选择房间";
        roomDescription.text = "";
        memberCount.text = "0" + "<size=32>/" + "0";
        levelName.text = "";
    }
}
