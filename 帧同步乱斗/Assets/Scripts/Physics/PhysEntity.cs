using BEPUutilities;
using FixMath.NET;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class PhysEntity : MonoBehaviour
{
    public BEPUphysics.Entities.Entity BEPUEntity;
    /// <summary>
    /// transform原点在物理形状坐标系下的偏移
    /// </summary>
    public BEPUutilities.Vector3 offset = BEPUutilities.Vector3.Zero;

    [SerializeField]
    protected BEPUutilities.Vector3 gravity = new BEPUutilities.Vector3(0, -9.81f, 0);
    [SerializeField]
    protected bool isKinematic = false;
    [SerializeField]
    protected Fix64 mass = Fix64.One;
    [SerializeField]
    protected Fix64 linearDamping = 0.05f;
    [SerializeField]
    protected Fix64 angularDumping = 0.05f;


    //设置
    static float positionSmoothSpeed = 10f;
    static float rotationSmoothSpeed = 10f;


    [Header("属性显示")]
    public BEPUutilities.Vector3 position;
    public BEPUutilities.Vector3 transformPosition;

    private void Awake()
    {
        BEPUEntity = ConstructPhysEntity();
        InitPhysEntity(BEPUEntity);
    }
    private void OnEnable()
    {
        PhysicsMgr.Instance.AddPhysicsEntity(BEPUEntity);
    }
    private void LateUpdate()
    {
        RenderMove();
        //显示
        position = BEPUEntity.Position;
        transformPosition = BEPUEntity.Position + GetWorldOffset();
    }
    private void OnDisable()
    {
        PhysicsMgr.Instance.RemovePhysicsEntity(BEPUEntity);
    }


    /// <summary>
    /// 构造物理实体
    /// </summary>
    protected abstract BEPUphysics.Entities.Entity ConstructPhysEntity();
    //公共部分初始化
    void InitPhysEntity(BEPUphysics.Entities.Entity physEntity)
    {
        //阻尼
        physEntity.LinearDamping = linearDamping;
        physEntity.AngularDamping = angularDumping;
        //重力
        physEntity.Gravity = gravity;
        //质量
        physEntity.Mass = mass;
        //是否运动学
        if (isKinematic) physEntity.BecomeKinematic();
        else physEntity.BecomeDynamic(mass);
        //位置和旋转
        physEntity.Position = transform.position.ToVector3();
        physEntity.Orientation = transform.rotation.ToBEPUQuaternion();
        //碰撞tag
        physEntity.CollisionInformation.Tag = gameObject.layer;
    }

    //渲染层平滑插值更新
    void RenderMove()
    {
        transform.position = UnityEngine.Vector3.Lerp(transform.position, (BEPUEntity.Position + GetWorldOffset()).ToVector3(), Time.deltaTime * positionSmoothSpeed);
        transform.rotation = UnityEngine.Quaternion.Lerp(transform.rotation, BEPUEntity.Orientation.ToUnityQuaternion(), Time.deltaTime * rotationSmoothSpeed);
    }

    public BEPUutilities.Vector3 GetWorldOffset()
    {
        return BEPUEntity.WorldTransform.Right * offset.X + BEPUEntity.WorldTransform.Up * offset.Y + BEPUEntity.WorldTransform.Forward * offset.Z;
    }

    public BEPUutilities.Vector3 GetTransformPosition()
    {
        return BEPUEntity.Position + GetWorldOffset();
    }
}
