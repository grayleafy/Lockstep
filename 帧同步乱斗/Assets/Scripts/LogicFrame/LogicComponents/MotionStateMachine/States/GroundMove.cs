using FixMath.NET;
using Messages.NetVector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GroundMove : MotionState
{
    public GroundCaster groundCaster;

    public Fix64 moveSpeed;

    //私有
    Animator animator;
    CommandRecorder commandRecorder;
    PhysEntity physEntity;
    CharacterConstrant characterConstrant;

    public override void OnEnter(MotionStateMachine stateMachine)
    {
        base.OnEnter(stateMachine);
        animator = stateMachine.GetComponent<Animator>();
        commandRecorder = stateMachine.GetComponent<CommandRecorder>();
        physEntity = stateMachine.GetComponent<PhysEntity>();
        characterConstrant = stateMachine.GetComponent<CharacterConstrant>();
    }

    public override StateUpdateResult OnLogicUpdate(MotionStateMachine stateMachine, Fix64 dt)
    {
        SetMotorPositionGoal(dt);
        SetMotorRotation(dt);
        return StateUpdateResult.Running;
    }

    public override void OnRenderUpdate(MotionStateMachine stateMachine)
    {
        base.OnRenderUpdate(stateMachine);
        SetAnimator();
    }

    //根据指令得到下一帧的水平偏移
    BEPUutilities.Vector3 GetMoveOffset(Fix64 dt)
    {
        return commandRecorder.command.moveCommand.ToBEPUVector3() * moveSpeed * dt;
    }
    //贴地偏移
    BEPUutilities.Vector3 GetStickGroundOffset()
    {
        if (groundCaster.isOnGround == false)
        {
            return BEPUutilities.Vector3.Zero;
        }

        return groundCaster.hitInfo.HitData.Location - (physEntity.BEPUEntity.Position + physEntity.GetWorldOffset());
    }

    //根据移动和贴地，设置电机的位移
    void SetMotorPositionGoal(Fix64 dt)
    {
        BEPUutilities.Vector3 posDetal = GetMoveOffset(dt) + GetStickGroundOffset();
        characterConstrant.SetGoal(posDetal + physEntity.GetTransformPosition());
    }
    //设置电机朝向
    void SetMotorRotation(Fix64 dt)
    {
        if (commandRecorder.command.moveCommand != NetVector3.zero)
        {
            Fix64 z = BEPUutilities.Vector3.Dot(commandRecorder.command.moveCommand.ToBEPUVector3(), BEPUutilities.Vector3.Forward);
            Fix64 x = BEPUutilities.Vector3.Dot(commandRecorder.command.moveCommand.ToBEPUVector3(), BEPUutilities.Vector3.Right);
            Fix64 pitch = Fix64.Atan2(x, -z);
            var goal = BEPUutilities.Quaternion.CreateFromAxisAngle(new BEPUutilities.Vector3(0, 1, 0), pitch);
            characterConstrant.SetGoal(goal);
        }
    }

    //设置动画
    void SetAnimator()
    {
        //移动速度
        Vector3 moveDir = commandRecorder.command.moveCommand.ToBEPUVector3().ToVector3() * (float)moveSpeed;
        float vertical = Vector3.Dot(physEntity.transform.forward, moveDir);
        float horizontal = Vector3.Dot(physEntity.transform.right, moveDir);

        animator.SetFloat("Horizontal", horizontal, 0.2f, Time.deltaTime);
        animator.SetFloat("Vertical", vertical, 0.2f, Time.deltaTime);
    }
}
