using Messages.LogicFrame;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LogicEntity))]
public class CommandRecorder : LogicComponent
{
    public EntityCommand command = new();

    public void ApplyNextCommand(EntityCommand command)
    {
        this.command.moveCommand = command.moveCommand;
        this.command.actionCommands.AddRange(command.actionCommands);
    }
}
