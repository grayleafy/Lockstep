using BattleServer.Player;
using LeafNet;
using LeafNet.Base;
using LeafNet.LeafNet;
using Messages.LogicFrame;
using Messages.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleServer.Battle
{
    public class BattleMgr : SingletonBase<BattleMgr>
    {
        //房间名和战斗的对应
        Dictionary<string, (Battle battle, BattleCache battleCache)> runningBattleInfos = new Dictionary<string, (Battle, BattleCache)>();

        public void Init()
        {
            NetServiceMgr.Instance.AddService<BattleStartReq>(OnBallteStartReq);
            NetServiceMgr.Instance.AddService<LevelInitFinishedReq>(OnLevelInitFinishReq);
            NetServiceMgr.Instance.AddService<FrameUpdateReq>(OnFrameUpdateReq);
            NetServiceMgr.Instance.AddService<ReconnectReq>(OnReconnectReq);
            NetServiceMgr.Instance.AddService<ReconnectLevelLoadFinishReq>(OnReconnectLevelLoadFinishReq);
        }

        #region 服务
        //战斗开始请求
        void OnBallteStartReq(BattleStartReq req, Session session, NetworkEntity networkEntity)
        {
            //找到房主
            ServerPlayer owner = PlayerMgr.GetInstance().GetPlayer(session);
            //房间
            ServerRoom room = owner.room;

            //开始战斗
            StartBattle(room);

            //发送协议让成员开始关卡初始化
            RoomMgr.GetInstance().Broadcast(new BattleStartRes() { battle = runningBattleInfos[room.name].battle }, room, null, networkEntity);
        }
        //关卡初始化完成请求
        void OnLevelInitFinishReq(LevelInitFinishedReq req, Session session, NetworkEntity networkEntity)
        {
            //玩家
            ServerPlayer player = PlayerMgr.GetInstance().GetPlayer(session);
            //房间
            ServerRoom room = player.room;

            //战斗信息
            (Battle battle, BattleCache battleCache) = runningBattleInfos[room.name];

            //找到玩家的序号， 设置完成标记
            SetReady(battle, battleCache, player);

            //有人还在加载
            if (CheckReady(battle, battleCache) == false) return;

            //重载就绪状态
            ResetReady(battle, battleCache);

            //全部加载完毕,广播协议
            RoomMgr.GetInstance().Broadcast(new LevelInitFinishedRes(), room, null, networkEntity);
            //广播第一帧
            battle.AddNextEmptyFrame();
            BroadcastFrame(battle, battleCache, networkEntity);
        }
        //帧同步协议
        void OnFrameUpdateReq(FrameUpdateReq req, Session session, NetworkEntity networkEntity)
        {
            //玩家
            ServerPlayer player = PlayerMgr.GetInstance().GetPlayer(session);
            //房间
            ServerRoom room = player.room;
            //战斗已结束
            if (runningBattleInfos.ContainsKey(room.name) == false) return;
            //战斗信息
            (Battle battle, BattleCache battleCache) = runningBattleInfos[room.name];


            //检测帧号是否一致
            if (req.frameIndex != battle.frameCommands.Count - 1)
            {
                NetLogger.GetInstance().Log("丢弃操作: 玩家名：" + player.name + "  帧id：" + req.frameIndex + "  当前服务器帧id:" + battle.frameCommands.Count);
                return;
            }

            //添加操作
            battle.frameCommands.Last().commands.AddRange(req.commands);

            //设置就绪
            SetReady(battle, battleCache, player);
        }
        //断线重连
        void OnReconnectReq(ReconnectReq req, Session session, NetworkEntity netEntity)
        {
            //玩家
            ServerPlayer player = PlayerMgr.GetInstance().GetPlayer(session);
            //房间
            ServerRoom room = player.room;
            //战斗信息
            (Battle battle, BattleCache battleCache) = runningBattleInfos[room.name];

            //发送战斗基本信息
            Battle battleInfo = battle.CopyInfo();

            ReconnectRes res = new ReconnectRes();
            res.battleInfo = battleInfo;

            netEntity.SendMessage(res, player.session);
        }
        //断线重连关卡加载完成
        void OnReconnectLevelLoadFinishReq(ReconnectLevelLoadFinishReq req, Session session, NetworkEntity networkEntity)
        {
            //玩家
            ServerPlayer player = PlayerMgr.GetInstance().GetPlayer(session);
            //房间
            ServerRoom room = player.room;
            //战斗信息
            (Battle battle, BattleCache battleCache) = runningBattleInfos[room.name];

            //每一条消息传输的帧数
            int frameSize = 5;
            List<FrameCommand> tempCommands = new List<FrameCommand>();
            foreach (FrameCommand frame in battle.frameCommands)
            {
                if (frame == battle.frameCommands.Last())
                {
                    break;
                }
                tempCommands.Add(frame);
                if (tempCommands.Count >= frameSize)
                {
                    ReconnectLevelLoadFinishRes res = new ReconnectLevelLoadFinishRes();
                    res.msgEnd = false;
                    res.frameCommands = tempCommands;
                    networkEntity.SendMessage(res, player.session);
                    tempCommands.Clear();
                }
            }
            {
                ReconnectLevelLoadFinishRes res = new ReconnectLevelLoadFinishRes();
                res.msgEnd = true;
                res.frameCommands = tempCommands;
                networkEntity.SendMessage(res, player.session);
                tempCommands.Clear();
            }

        }
        #endregion


        Battle CreateBattle(ServerRoom room)
        {
            Battle battle = new Battle();
            battle.roomName = room.name;
            battle.owner = room.owner;
            battle.levelId = room.levelId;
            battle.members = new List<Messages.SharedData.RoomPlayer>(room.members.ToArray());
            return battle;
        }

        void StartBattle(ServerRoom room)
        {
            Battle battle = CreateBattle(room);
            runningBattleInfos.Add(room.name, (battle, new BattleCache(battle)));

            //所有玩家的状态改变
            foreach (var player in battle.members)
            {
                PlayerMgr.GetInstance().GetPlayer(player.name).playState = GameState.InGaming;
            }
        }


        //广播帧同步协议
        void BroadcastFrame(Battle battle, BattleCache battleCache, NetworkEntity networkEntity)
        {
            //战斗是否结束
            if (runningBattleInfos.ContainsKey(battle.roomName) == false) return;

            //广播协议
            RoomMgr.GetInstance().Broadcast(new FrameUpdateRes() { frameCommand = battle.frameCommands.Last() }, RoomMgr.GetInstance().GetRoom(battle.roomName), null, networkEntity);

            //重载就绪
            ResetReady(battle, battleCache);

            //添加下一帧
            battle.AddNextEmptyFrame();



            //下一次帧同步的定时器
            Action<NetworkEntity> task = (net) =>
            {
                BroadcastFrame(battle, battleCache, networkEntity);
            };
            //定时器
            ServerTimer.GetInstance().AddTask(new TimerTask() { waitTime = 50, loop = false, callback = task });
        }


        #region 就绪相关
        //设置就绪情况
        void SetReady(Battle battle, BattleCache battleCache, ServerPlayer player)
        {
            //找到玩家的序号， 设置完成标记
            for (int i = 0; i < battle.members.Count; i++)
            {
                if (player.name == battle.members[i].name)
                {
                    battleCache.memberIsReady[i] = true;
                    break;
                }
            }
        }
        //全部重载就绪状态
        void ResetReady(Battle battle, BattleCache battleCache)
        {
            for (int i = 0; i < battle.members.Count; i++)
            {
                battleCache.memberIsReady[i] = false;
            }
        }
        //是否所有成员已就绪
        bool CheckReady(Battle battle, BattleCache battleCache)
        {
            //有人还在加载
            for (int i = 0; i < battleCache.memberIsReady.Count; i++)
            {
                if (battleCache.memberIsReady[i] == false)
                {
                    return false;
                }
            }
            return true;
        }
        #endregion
    }
}
