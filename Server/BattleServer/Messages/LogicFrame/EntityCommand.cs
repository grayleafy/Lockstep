using Messages.NetVector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messages.LogicFrame
{

    public enum InputType
    {
        Tap,
        Hold,
    }

    [Serializable]
    public class ActionCommand
    {
        public string key;
        public InputType inputType;
    }

    /// <summary>
    /// 一个实体的一个逻辑帧的操作
    /// </summary>

    [Serializable]
    public class EntityCommand
    {
        public int entityId;
        public NetVector3 moveCommand = NetVector3.zero;
        public List<ActionCommand> actionCommands = new List<ActionCommand>();

        /// <summary>
        /// 是否未进行任何操作
        /// </summary>
        /// <returns></returns>
        public bool IsNoOperator()
        {
            return moveCommand == NetVector3.zero && actionCommands.Count == 0;
        }

        public void Reset()
        {
            moveCommand = NetVector3.zero;
            actionCommands.Clear();
        }
    }
}
