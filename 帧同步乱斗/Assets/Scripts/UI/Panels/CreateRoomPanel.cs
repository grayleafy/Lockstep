using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreateRoomPanel : BasePanel
{
    public Button createButton;
    public Button cancelButton;
    public TMP_InputField roomName;
    public TMP_InputField roomDescription;

    public override void OnShow()
    {
        base.OnShow();
        createButton.onClick.AddListener(CreateRoomBegin);
        cancelButton.onClick.AddListener(Cancel);
        EventCenter.Instance.AddEventListener(EventName.CreateRoomSuccess, CreateRoomBeginSuccess);
    }

    public override void OnStartHide()
    {
        base.OnStartHide();
        createButton.onClick.RemoveAllListeners();
        cancelButton.onClick.RemoveAllListeners();
        EventCenter.Instance.RemoveEventListener(EventName.CreateRoomSuccess, CreateRoomBeginSuccess);
    }

    void Cancel()
    {
        Hide();
        UIMgr.Instance.ShowPanel<JoinOrCreateRoomPanel>("JoinOrCreateRoomPanel");
    }

    void CreateRoomBegin()
    {
        RoomMgr.Instance.CreateRoomBegin(roomName.text, roomDescription.text);
    }

    void CreateRoomBeginSuccess()
    {
        Hide();
        UIMgr.Instance.ShowPanel<RoomPanel>("RoomPanel");
    }
}
