using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LogType
{
    ManagerInit,
    NetService,
    ClassCast,
    LogicFrame,
    Level,
    LogicEntity,
    Temp,

}

public class Logger : Singleton<Logger>
{
    private Dictionary<LogType, bool> masks;

    public Logger()
    {
        masks = new Dictionary<LogType, bool>();
        foreach (LogType logType in Enum.GetValues(typeof(LogType)))
        {
            masks.Add(logType, true);
        }
    }

    public void SetMask(LogType logType, bool value)
    {
        masks[logType] = value;
    }

    public void Log(LogType logType, object message)
    {
        if (masks[logType])
        {
            Debug.Log("Logger: LogType: " + logType.ToString() + "   Message: " + message);
        }
    }

    public void LogError(LogType logType, object message)
    {
        if (masks[logType])
        {
            Debug.LogError("Logger: LogType: " + logType.ToString() + "   Message: " + message);
        }
    }
}
