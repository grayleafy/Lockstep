using BattleServer.Battle;
using FixMath.NET;
using LeafNet;
using Messages.LogicFrame;
using Messages.Messages;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum BattleStartState
{
    Success,
    MemberNotEnough,
    CharacterRepeat,
}

public enum BattleStatus
{
    Gaming,
    Reconnect,
}

public class BattleMgr : SingletonMono<BattleMgr>
{
    public Battle currentBattle;
    public int nextLogicFrameIndex = 0;
    BattleStatus status = BattleStatus.Gaming;

    #region 生命周期
    public void Awake()
    {
        NetServiceMgr.Instance.AddService<BattleStartRes>(OnBattleStartRes);
        NetServiceMgr.Instance.AddService<LevelInitFinishedRes>(OnLevelInitFinishedRes);
        NetServiceMgr.Instance.AddService<FrameUpdateRes>(OnFrameUpdateRes);
        NetServiceMgr.Instance.AddService<ReconnectRes>(OnReconnectRes);
        NetServiceMgr.Instance.AddService<ReconnectLevelLoadFinishRes>(OnReconnectLevelLoadFinishRes);
    }
    private void Update()
    {
        TryFrameUpdate();
    }
    #endregion



    #region 服务
    //战斗，开始初始化
    void OnBattleStartRes(BattleStartRes res, Session session, NetworkEntity networkEntity)
    {
        //保存战斗数据
        currentBattle = res.battle;

        //开始加载关卡,加载完成发送协议
        LevelMgr.Instance.loadLevelFinishEvent = () => NetMgr.Instance.SendMsg(new LevelInitFinishedReq());
        LevelMgr.Instance.LoadLevelAsync(currentBattle.levelId);
    }

    //所有人都加载完成
    void OnLevelInitFinishedRes(LevelInitFinishedRes res, Session session, NetworkEntity networkEntity)
    {
        Logger.Instance.Log(LogType.Level, "进入关卡");

        //帧同步开始
        ResetFrameSync();
    }
    //收到帧信息
    void OnFrameUpdateRes(FrameUpdateRes res, Session session, NetworkEntity networkEntity)
    {
        if (status == BattleStatus.Gaming)
        {
            currentBattle?.frameCommands.Add(res.frameCommand);
            Logger.Instance.Log(LogType.LogicFrame, "帧同步接收:" + res.frameCommand.frameIndex);
        }
        else
        {
            Logger.Instance.Log(LogType.LogicFrame, "帧同步接收但丢弃");
        }
    }
    #endregion







    #region 帧同步部分
    /// <summary>
    /// 重置帧同步
    /// </summary>
    public void ResetFrameSync()
    {
        nextLogicFrameIndex = 0;
        //设置玩家的输入组件
        for (int i = 0; i < currentBattle.members.Count; i++)
        {
            if (currentBattle.members[i].name == PlayerMgr.Instance.hostPlayer.name)
            {
                Level level = LevelMgr.Instance.GetLevel(currentBattle.levelId);
                GameObject character = LevelMgr.Instance.FindPlayerCharacter(level.characters[currentBattle.members[i].characterId].fullName);
                character.FindInFirstChildren("Entity").AddComponent<CommandInputer>();
                character.FindInFirstChildren("Camera").SetActive(true);
            }
        }

    }
    //读取帧，如果有则更新逻辑帧
    void TryFrameUpdate()
    {
        if (currentBattle != null)
        {
            //帧号对应则取出
            while (currentBattle.frameCommands.Count > 0 && currentBattle.frameCommands.First().frameIndex == nextLogicFrameIndex)
            {
                //取出
                nextLogicFrameIndex++;
                FrameCommand frameCommand = currentBattle.frameCommands.First();
                currentBattle.frameCommands.Remove(frameCommand);

                Logger.Instance.Log(LogType.LogicFrame, "帧处理" + frameCommand.frameIndex);

                //发送自己这一帧内的操作输入
                SendFrameCommand();

                //应用已经同步过的操作
                ApplyFrameCommand(frameCommand);

                //帧更新
                LogicFrameMgr.Instance.LogicFrameUpdate(PhysicsMgr.Instance.tickStep);
            }
        }
    }
    #endregion


    //应用收到的指令
    void ApplyFrameCommand(FrameCommand frameCommand)
    {
        CommandMgr.Instance.ApplyCommand(frameCommand.commands);
    }
    //往服务器发送自己的操作
    void SendFrameCommand()
    {
        var commands = CommandMgr.Instance.GetChangeEntityCommand();
        FrameUpdateReq req = new FrameUpdateReq();
        req.frameIndex = nextLogicFrameIndex;
        req.commands = commands;
        NetMgr.Instance.SendMsg(req);
        Logger.Instance.Log(LogType.LogicFrame, "帧同步发送:" + nextLogicFrameIndex);

        //发送完清空
        CommandMgr.Instance.ResetCommandInputers();
    }



    /// <summary>
    /// 发送开始战斗的协议
    /// </summary>
    public BattleStartState BattleStart(Room room)
    {
        //合法性判断
        //成员不够
        if (room.MaxMember > room.Members.Count)
        {
            return BattleStartState.MemberNotEnough;
        }

        //角色重复
        HashSet<int> existIds = new HashSet<int>();
        for (int i = 0; i < room.Members.Count; i++)
        {
            if (existIds.Contains(room.Members[i].CharacterId))
            {
                return BattleStartState.CharacterRepeat;
            }
            existIds.Add(room.Members[i].CharacterId);
        }

        //发送请求
        NetMgr.Instance.SendMsg(new BattleStartReq());
        return BattleStartState.Success;
    }


    #region 断线重连
    public void BeginReConnect()
    {
        //关闭ui
        string[] pannels = UIMgr.Instance.uniquePanelDic.Keys.ToArray();
        foreach (var panel in pannels)
        {
            UIMgr.Instance.uniquePanelDic[panel].Hide();
        }

        status = BattleStatus.Reconnect;

        //发送协议
        NetMgr.Instance.SendMsg(new ReconnectReq());
    }
    void OnReconnectRes(ReconnectRes res, Session session, NetworkEntity networkEntity)
    {
        //保存战斗数据
        currentBattle = res.battleInfo;

        //开始加载关卡
        LevelMgr.Instance.loadLevelFinishEvent = () =>
        {
            NetMgr.Instance.SendMsg(new ReconnectLevelLoadFinishReq());
            ResetFrameSync();
        };
        LevelMgr.Instance.LoadLevelAsync(currentBattle.levelId);
    }
    //收到帧信息
    void OnReconnectLevelLoadFinishRes(ReconnectLevelLoadFinishRes res, Session session, NetworkEntity networkEntity)
    {
        for (int i = 0; i < res.frameCommands.Count; i++)
        {
            currentBattle.frameCommands.Add(res.frameCommands[i]);
        }

        if (res.msgEnd)
        {
            status = BattleStatus.Gaming;
            Logger.Instance.Log(LogType.LogicFrame, "断线重连接收帧信息完成");
        }
    }
    #endregion
}
