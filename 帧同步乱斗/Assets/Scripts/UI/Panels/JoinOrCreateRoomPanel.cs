using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JoinOrCreateRoomPanel : BasePanel
{
    public Button joinButton;
    public Button createButton;

    public override void OnShow()
    {
        base.OnShow();
        joinButton.onClick.AddListener(JoinRoom);
        createButton.onClick.AddListener(CreateRoom);
    }

    public override void OnStartHide()
    {
        base.OnStartHide();
        joinButton.onClick.RemoveListener(JoinRoom);
        createButton.onClick.RemoveListener(CreateRoom);
    }

    void JoinRoom()
    {
        Hide();
        UIMgr.Instance.ShowPanel<RoomSelectPanel>("RoomSelectPanel");
    }

    void CreateRoom()
    {
        Hide();
        UIMgr.Instance.ShowPanel<CreateRoomPanel>("CreateRoomPanel");
    }
}
