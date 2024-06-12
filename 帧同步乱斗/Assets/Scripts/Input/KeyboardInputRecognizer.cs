using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class KeyboardInputCommand
{
    //public static float holdTime = 0.5f;
    public enum InputType
    {
        Tap,
        Hold,
        Unknown
    };

    public KeyCode key;
    public InputType inputType = InputType.Unknown;
    [HideInInspector]
    public float pressTime;
    [HideInInspector]
    public float unPressTime = -1;

    /// <summary>
    /// 是否一致
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    public bool IsMatch(KeyboardInputCommand command)
    {
        return key == command.key && inputType == command.inputType;
    }

    /// <summary>
    /// 根据按下和放开的时间计算输入种类
    /// </summary>
    //public void SetInputType()
    //{
    //    if (unPressTime - pressTime >= holdTime)
    //    {
    //        inputType = InputType.Hold;
    //    }
    //    else
    //    {
    //        inputType = InputType.Tap;
    //    }
    //}
}

/// <summary>
/// 负责区分键盘的短按和长按操作
/// </summary>
[Serializable]
public class KeyboardInputRecognizer
{
    public bool canInput = true;   //是否接受输入
    public float timeout = 0.5f;
    public float holdTime = 0.5f;



    Dictionary<KeyCode, bool> keyDic = new Dictionary<KeyCode, bool>();
    Dictionary<KeyCode, KeyboardInputCommand> commandDic = new();//已经按下还未松开的键
    Queue<KeyboardInputCommand> commandQueue = new();
    public enum TapType
    {
        ShortTap,
        LongTap
    }

    public void OnUpdate()
    {
        DetectHold();
        ClearTimeoutCommand();
    }

    /// <summary>
    /// 清理输入
    /// </summary>
    public void ClearInput()
    {
        keyDic.Clear();
        commandQueue.Clear();
        commandDic.Clear();
    }
    /// <summary>
    /// 当前按下的所有按键
    /// </summary>
    /// <returns></returns>
    public List<KeyCode> GetDownKeys()
    {
        return commandDic.Keys.ToList();
    }




    #region 外部输入操作调用
    /// <summary>
    /// 获取下一个指令并且移出队列
    /// </summary>
    /// <returns></returns>
    public KeyboardInputCommand DequeueInputCommand()
    {
        if (commandQueue.Count == 0)
        {
            return null;
        }
        return commandQueue.Dequeue(); ;
    }
    public void PressKey(KeyCode key)
    {
        if (canInput == true)
        {
            keyDic[key] = true;
            //Debug.Log("press " + key);
            KeyboardInputCommand inputCommand = new KeyboardInputCommand()
            {
                key = key,
                pressTime = Time.realtimeSinceStartup
            };
            commandDic[key] = inputCommand;
        }
    }

    public void UnPressKey(KeyCode key)
    {
        if (canInput == true)
        {
            keyDic[key] = false;
            //Debug.Log("unpress " + key);
            if (commandDic.ContainsKey(key))
            {
                var command = commandDic[key];
                command.unPressTime = Time.realtimeSinceStartup;
                //如果队列中还不存在该命令则加入指令队列
                if (command.inputType == KeyboardInputCommand.InputType.Unknown)
                {
                    if (command.unPressTime - command.pressTime >= holdTime)
                    {
                        command.inputType = KeyboardInputCommand.InputType.Hold;
                    }
                    else
                    {
                        command.inputType = KeyboardInputCommand.InputType.Tap;
                    }
                    commandQueue.Enqueue(command);
                    commandDic.Remove(key);
                }
            }
        }
    }

    public void TapKey(KeyCode key)
    {
        PressKey(key);
        UnPressKey(key);
    }

    public bool GetKey(KeyCode key)
    {
        if (keyDic.ContainsKey(key) == false)
        {
            keyDic.Add(key, false);
        }
        return keyDic[key];
    }

    public void HoldKey(KeyCode key, float keepTime)
    {
        //协程实现
        throw new System.NotImplementedException();
    }
    #endregion




    //清理过期的指令
    void ClearTimeoutCommand()
    {
        while (commandQueue.Count > 0)
        {
            KeyboardInputCommand command = commandQueue.Peek();
            if (command.unPressTime >= 0 && command.pressTime < Time.realtimeSinceStartup - timeout)
            {
                commandQueue.Dequeue();
            }
            else
            {
                break;
            }
        }
    }

    //检测Hold
    void DetectHold()
    {
        float currentTime = Time.realtimeSinceStartup;
        foreach (var key in commandDic.Keys)
        {
            if (commandDic[key].inputType == KeyboardInputCommand.InputType.Unknown && currentTime - commandDic[key].pressTime >= holdTime)
            {
                commandDic[key].inputType = KeyboardInputCommand.InputType.Hold;
                commandQueue.Enqueue(commandDic[key]);
            }
        }
    }

    /// <summary>
    /// 由技能系统调用，忽略不需要的指令
    /// </summary>
    /// <param name="validKeys"></param>
    public void IgnoreInvalidTap(HashSet<KeyCode> validKeys)
    {
        throw new System.NotImplementedException();
    }

}
