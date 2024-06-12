using Messages.LogicFrame;
using System.Collections.Generic;
using UnityEngine;

public class CommandMgr : Singleton<CommandMgr>
{
    List<CommandInputer> commandInputers = new List<CommandInputer>();
    //登录
    public void CommandUnputerLogin(CommandInputer commandInputer)
    {
        commandInputers.Add(commandInputer);
    }
    //注销
    public void CommandInputerLogout(CommandInputer commandInputer)
    {
        commandInputers.Remove(commandInputer);
    }
    /// <summary>
    /// 获取这一帧内有操作的输入
    /// </summary>
    /// <returns></returns>
    public List<EntityCommand> GetChangeEntityCommand()
    {
        List<EntityCommand> entityCommands = new List<EntityCommand>();
        foreach (CommandInputer commandInputer in commandInputers)
        {
            //if (commandInputer.commandCache.IsNoOperator() == false)
            //{
            entityCommands.Add(commandInputer.commandCache);
            //commandInputer.commandCache.Reset();
            //}
        }


        return entityCommands;
    }
    /// <summary>
    /// 清空所有的输入器
    /// </summary>
    public void ResetCommandInputers()
    {
        foreach (CommandInputer commandInputer in commandInputers)
        {
            commandInputer.ResetCommandCache();
        }
    }

    /// <summary>
    /// 找到对应的物体，然后应用操作
    /// </summary>
    public void ApplyCommand(List<EntityCommand> entityCommands)
    {
        for (int i = 0; i < entityCommands.Count; i++)
        {
            LogicEntity entity = LogicFrameMgr.Instance.GetLogicEntity(entityCommands[i].entityId);
            CommandRecorder commandRecorder = entity.GetComponent<CommandRecorder>();
            commandRecorder.ApplyNextCommand(entityCommands[i]);
            //Debug.Log("id = " + entityCommands[i].entityId + "move = " + entityCommands[i].moveCommand.ToBEPUVector3().ToVector3());
        }
    }
}
