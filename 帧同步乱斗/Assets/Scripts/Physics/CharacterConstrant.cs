using FixMath.NET;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 角色角度和位置约束，使用电机驱动,需要在物理实体运行结束之后运行，脚本顺序
/// </summary>
public class CharacterConstrant : MonoBehaviour
{
    private PhysEntity physEntity;
    public PhysEntity PhysEntity
    {
        get
        {
            if (physEntity == null)
            {
                physEntity = GetComponent<PhysEntity>();
            }
            return physEntity;
        }
    }

    public BEPUphysics.Constraints.SingleEntity.SingleEntityLinearMotor singleEntityLinearMotor;
    public BEPUphysics.Constraints.SingleEntity.SingleEntityAngularMotor singleEntityAngularMotor;


    private void Awake()
    {
        singleEntityLinearMotor = new BEPUphysics.Constraints.SingleEntity.SingleEntityLinearMotor(PhysEntity.BEPUEntity, PhysEntity.BEPUEntity.Position);
        singleEntityAngularMotor = new BEPUphysics.Constraints.SingleEntity.SingleEntityAngularMotor(PhysEntity.BEPUEntity);
        InitMotor();
    }
    private void OnEnable()
    {
        PhysicsMgr.Instance.space.Add(singleEntityLinearMotor);
        PhysicsMgr.Instance.space.Add(singleEntityAngularMotor);
    }
    private void OnDisable()
    {
        PhysicsMgr.Instance.space.Remove(singleEntityLinearMotor);
        PhysicsMgr.Instance.space.Remove(singleEntityAngularMotor);
    }


    void InitMotor()
    {
        singleEntityLinearMotor.Settings.Mode = BEPUphysics.Constraints.TwoEntity.Motors.MotorMode.Servomechanism;
        singleEntityLinearMotor.Settings.Servo.Goal = transform.position.ToVector3();

        singleEntityAngularMotor.Settings.Mode = BEPUphysics.Constraints.TwoEntity.Motors.MotorMode.Servomechanism;
        singleEntityAngularMotor.Settings.Servo.Goal = transform.rotation.ToBEPUQuaternion();
    }


    public void SetGoal(BEPUutilities.Vector3 goal)
    {
        goal = goal - PhysEntity.GetWorldOffset();
        singleEntityLinearMotor.Settings.Servo.Goal = goal;
    }

    public void SetGoal(BEPUutilities.Quaternion goal)
    {
        singleEntityAngularMotor.Settings.Servo.Goal = goal;
    }
}
