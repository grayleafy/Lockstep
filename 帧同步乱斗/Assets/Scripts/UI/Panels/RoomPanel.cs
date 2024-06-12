using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomPanel : BasePanel, IView
{
    Room room;
    IModel IView.Data { get => room; set => room = value as Room; }

    public TextMeshProUGUI roomName;
    public TextMeshProUGUI memberCount;

    //房间成员
    public ScrollList memberScroll;
    //选择角色
    public LoopScrollList characterSelect;
    private ModelList<CharacterInfo> characterInfos = new ModelList<CharacterInfo>();

    [Header("按钮")]
    public Button startBattleButton;
    public Button backButton;
    public Button rightCharacterButton;
    public Button leftCharacterButton;

    public void RefreshView(IModel data)
    {
        roomName.text = room.Name;
        memberCount.text = room.CurrentMemberCount.ToString() + "<size=32>/" + room.MaxMember.ToString();

        BindMemberList();
    }

    public override void OnShow()
    {
        base.OnShow();
        (this as IView).BindData(RoomMgr.Instance.currentRoom);

        characterInfos.Reset(LevelMgr.Instance.GetLevel(room.LevelId).characters, (characterInfo) => characterInfo);
        characterSelect.BindData(characterInfos);
        characterSelect.SelectModifiedEvent += ChangeCharacter;

        //按钮
        backButton.onClick.AddListener(() =>
        {
            Hide();
            UIMgr.Instance.ShowPanel<JoinOrCreateRoomPanel>("JoinOrCreateRoomPanel");
        });
        startBattleButton.onClick.AddListener(() =>
        {
            var result = BattleMgr.Instance.BattleStart(room);
            if (result == BattleStartState.Success)
            {

            }
            else if (result == BattleStartState.CharacterRepeat)
            {
                UIMgr.Instance.ShowPanel<TooltipPanel>("TooltipPanel", (panel) => panel.SetTooltip("开始游戏失败", "玩家选择的角色存在重复"));
            }
            else if (result == BattleStartState.MemberNotEnough)
            {
                UIMgr.Instance.ShowPanel<TooltipPanel>("TooltipPanel", (panel) => panel.SetTooltip("开始游戏失败", "成员不足"));
            }
        });
        leftCharacterButton.onClick.AddListener(() => characterSelect.SetSelectIndexSlowly((characterSelect.CurrentSelectRealIndex - 1 + characterInfos.Count) % characterInfos.Count, -1));
        rightCharacterButton.onClick.AddListener(() => characterSelect.SetSelectIndexSlowly((characterSelect.CurrentSelectRealIndex + 1 + characterInfos.Count) % characterInfos.Count, 1));

        //开始加载关卡时应该关闭自己
        EventCenter.Instance.AddEventListener(EventName.LevelLoadStart, HideWhenLoadLevel);
    }

    public override void OnStartHide()
    {
        base.OnStartHide();
        (this as IView).UnbindData();

        UnbindMemberList();

        characterSelect.UnbindData();
        characterSelect.SelectModifiedEvent -= ChangeCharacter;

        //按钮
        backButton.onClick.RemoveAllListeners();
        startBattleButton.onClick.RemoveAllListeners();
        leftCharacterButton.onClick.RemoveAllListeners();
        rightCharacterButton.onClick.RemoveAllListeners();

        ModelRenderMgr.Instance.ClearAll();

        //开始加载关卡时应该关闭自己
        EventCenter.Instance.RemoveEventListener(EventName.LevelLoadStart, HideWhenLoadLevel);
    }

    void BindMemberList()
    {
        UnbindMemberList();
        memberScroll.BindData(room.Members);
    }

    void UnbindMemberList()
    {
        memberScroll.UnbindData();
    }

    //改变角色
    void ChangeCharacter(int oldIndex, int newIndex)
    {
        RoomMgr.Instance.ChangeCharacter(newIndex);
    }

    /// <summary>
    /// 开始加载关卡
    /// </summary>
    void HideWhenLoadLevel()
    {
        Hide();
    }
}
