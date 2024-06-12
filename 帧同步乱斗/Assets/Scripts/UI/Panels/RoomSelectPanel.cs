using Messages.Messages;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomSelectPanel : BasePanel
{
    public ScrollList scrollList;
    public HighLightRoomPanel highLightRoomPanel;
    public TMP_InputField searchInput;
    public Button searchButton;
    public Button joinButton;

    private ModelList<Room> displayRoom;

    public override void OnShow()
    {
        base.OnShow();
        //按钮事件
        searchButton.onClick.AddListener(SearchRoom);
        joinButton.onClick.AddListener(JoinRoom);

        //滚动区域
        scrollList.deselectEvent += CancelSelectRoom;
        scrollList.selectEvent += HighlightRoomChange;

        //高亮区域
        highLightRoomPanel.SetEmpty();

        //绑定房间列表
        BindData(RoomMgr.Instance.allRooms);

        //刷新房间列表
        SearchRoom();

        //加入房间事件绑定
        EventCenter.Instance.AddEventListener<JoinRoomState>(EventName.OnJoinRoom, OnJoinRoom);
    }

    public override void OnStartHide()
    {
        base.OnStartHide();
        //按钮事件
        searchButton.onClick.RemoveListener(SearchRoom);
        joinButton.onClick.RemoveListener(JoinRoom);

        scrollList.deselectEvent -= CancelSelectRoom;
        scrollList.selectEvent -= HighlightRoomChange;

        //取消绑定
        UnbindData();

        //取消事件监听
        EventCenter.Instance.RemoveEventListener<JoinRoomState>(EventName.OnJoinRoom, OnJoinRoom);
    }

    public void BindData(ModelList<Room> roomList)
    {
        UnbindData();
        displayRoom = roomList;
        scrollList.BindData(displayRoom);
    }

    public void UnbindData()
    {
        if (displayRoom != null)
        {
            scrollList.UnbindData();
        }
    }

    void SearchRoom()
    {
        RoomMgr.Instance.SeachRoomsBegin(searchInput.text);
    }


    void CancelSelectRoom(int index)
    {
        (highLightRoomPanel as IView).BindData(null);
    }

    //高亮的房间更改，不是房间内部的数据改变
    void HighlightRoomChange(int index)
    {
        if (index >= 0 && index < displayRoom.Count)
        {
            (highLightRoomPanel as IView).BindData(displayRoom[index]);
        }
        else
        {
            (highLightRoomPanel as IView).BindData(null);
        }
    }

    //加入房间
    void JoinRoom()
    {
        if (highLightRoomPanel.roomName.text == "")
        {
            UIMgr.Instance.ShowPanel<TooltipPanel>("TooltipPanel", (panel) =>
            {
                panel.SetTooltip("加入房间失败", "请选择房间");
            });
            return;
        }


        RoomMgr.Instance.JoinRoomBegin(highLightRoomPanel.roomName.text);
    }

    void OnJoinRoom(JoinRoomState state)
    {
        if (state == JoinRoomState.Success)
        {
            Hide();
            UIMgr.Instance.ShowPanel<RoomPanel>("RoomPanel");
        }

        else
        {
            UIMgr.Instance.ShowPanel<TooltipPanel>("TooltipPanel", (panel) =>
            {
                panel.SetTooltip("加入房间失败", state.ToString());
            });
        }
    }
}
