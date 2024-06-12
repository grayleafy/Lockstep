using Messages.Messages;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelMgr : Singleton<LevelMgr>
{
    private LevelConfig levelConfig = null;
    public Level currentLevel;
    public Action loadLevelFinishEvent;

    public LevelMgr()
    {
        //加载配置
        levelConfig = ABMgr.Instance.LoadRes<LevelConfig>("config", "LevelConfig");

        //事件设置
        EventCenter.Instance.AddEventListener(EventName.SceneLoadFinish, OnLevelLoadFinish);
    }

    public Level GetLevel(int levelId)
    {
        return levelConfig.levels[levelId];
    }

    /// <summary>
    /// 开始加载关卡
    /// </summary>
    /// <param name="levelId"></param>
    public void LoadLevelAsync(int levelId)
    {
        EventCenter.Instance.EventTrigger(EventName.LevelLoadStart);
        currentLevel = GetLevel(levelId);
        ScenesMgr.Instance.LoadSceneAsyn(currentLevel.name);
    }

    //关卡加载完成
    void OnLevelLoadFinish()
    {
        EventCenter.Instance.EventTrigger(EventName.LevelLoadFinish);

        loadLevelFinishEvent?.Invoke();
        ////发送协议，完成初始化
        //NetMgr.Instance.SendMsg(new LevelInitFinishedReq());
    }

    /// <summary>
    /// 找到玩家控制的角色
    /// </summary>
    /// <returns></returns>
    public GameObject FindPlayerCharacter(string characterName)
    {
        return GameObject.Find(characterName);
    }
}
