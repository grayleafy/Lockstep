using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RoomMemberPanel : BasePanel, IView
{
    private RoomMember roomMember;
    IModel IView.Data { get => roomMember; set => roomMember = value as RoomMember; }


    public TextMeshProUGUI memberName;
    public TextMeshProUGUI character;

    public void RefreshView(IModel data)
    {
        memberName.text = roomMember.Name;
        //角色名称
        var level = LevelMgr.Instance.GetLevel(RoomMgr.Instance.currentRoom.LevelId);
        character.text = level.characters[roomMember.CharacterId].fullName;
    }
}
