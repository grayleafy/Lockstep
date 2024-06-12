using LeafNet;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking.Match;





public class GameMain : SingletonMono<GameMain>
{

    void Start()
    {
        UIMgr.Instance.ShowPanel<TapPanel>("TapPanel");










    }

    /// <summary>
    /// 初始化
    /// </summary>
    public void Init()
    {
       //Logger.Instance.SetMask(LogType.LogicFrame, false);

        //管理器初始化
        Logger.Instance.Log(LogType.ManagerInit, PlayerMgr.Instance);
        Logger.Instance.Log(LogType.ManagerInit, LevelMgr.Instance);
        Logger.Instance.Log(LogType.ManagerInit, BattleMgr.Instance);
    }




    #region 测试

    #endregion
}
