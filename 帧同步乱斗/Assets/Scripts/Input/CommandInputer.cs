using Messages.LogicFrame;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(LogicEntity))]
[RequireComponent(typeof(CommandRecorder))]
public class CommandInputer : MonoBehaviour
{
    public LogicEntity logicEntity;
    //指令缓冲
    [SerializeField]
    public EntityCommand commandCache = new();
    //识别短按长按
    public KeyboardInputRecognizer keyboardInputRecognizer = new KeyboardInputRecognizer();

    #region 生命周期
    private void Awake()
    {
        CommandMgr.Instance.CommandUnputerLogin(this);
    }
    private void OnEnable()
    {
        keyboardInputRecognizer.canInput = true;
    }

    private void Start()
    {
        logicEntity = GetComponent<LogicEntity>();
        commandCache.entityId = logicEntity.id;
    }
    private void OnDisable()
    {
        keyboardInputRecognizer.canInput = false;
    }
    private void OnDestroy()
    {
        CommandMgr.Instance.CommandInputerLogout(this);
    }

    private void Update()
    {
        //用户输入进行更新
        InputMove();
        InputActionCommand();
        keyboardInputRecognizer.OnUpdate();
        //读取行动指令
        KeyboardInputCommand keyboardCommand;
        while ((keyboardCommand = keyboardInputRecognizer.DequeueInputCommand()) != null)
        {
            ActionCommand command = new ActionCommand();
            command.key = keyboardCommand.key.ToString();
            if (keyboardCommand.inputType == KeyboardInputCommand.InputType.Tap)
            {
                command.inputType = InputType.Tap;
            }
            else if (keyboardCommand.inputType == KeyboardInputCommand.InputType.Hold)
            {
                command.inputType = InputType.Hold;
            }
            commandCache.actionCommands.Add(command);
        }
    }
    #endregion

    /// <summary>
    /// 重置指令缓冲
    /// </summary>
    public void ResetCommandCache()
    {
        commandCache.Reset();
    }

    #region 用户键盘输入
    //移动输入
    void InputMove()
    {
        float up = Input.GetKey(KeyCode.W) == true ? 1.0f : 0f;
        float down = Input.GetKey(KeyCode.S) == true ? -1.0f : 0f;
        float left = Input.GetKey(KeyCode.A) == true ? -1.0f : 0f;
        float right = Input.GetKey(KeyCode.D) == true ? 1.0f : 0f;
        Vector2 move = new Vector2(left + right, up + down).normalized;

        Vector3 worldMove = TransformMoveDirection(move, Camera.main, out _);
        if (up == 0 && down == 0 && left == 0 && right == 0)
        {
            worldMove = Vector3.zero;
        }
        commandCache.moveCommand = worldMove.ToNetVector3();
    }

    //处理行动指令
    void InputActionCommand()
    {
        if (Input.anyKeyDown)
        {
            foreach (KeyCode key in Enum.GetValues(typeof(KeyCode)))
            {
                if (key == KeyCode.Mouse0 && EventSystem.current.IsPointerOverGameObject())
                {
                    continue;
                }
                if (Input.GetKeyDown(key))
                {
                    keyboardInputRecognizer.PressKey(key);
                }
            }
        }
        var downKeys = keyboardInputRecognizer.GetDownKeys();
        foreach (KeyCode key in downKeys)
        {
            if (key == KeyCode.Mouse0 && EventSystem.current.IsPointerOverGameObject())
            {
                continue;
            }
            if (Input.GetKeyUp(key))
            {
                keyboardInputRecognizer.UnPressKey(key);
            }
        }
    }
    #endregion



    #region 镜头转换
    /// <summary>
    /// 计算摄像头转换后的移动的世界方向
    /// </summary>
    /// <param name="moveInput"></param>
    /// <param name="camera"></param>
    /// <returns></returns>
    public Vector3 TransformMoveDirection(Vector2 moveInput, Camera camera, out Quaternion rotate)
    {
        Vector3 up = Vector3.ProjectOnPlane(camera.transform.up, Vector3.up).normalized;
        Vector3 right = Vector3.ProjectOnPlane(camera.transform.right, Vector3.up).normalized;
        Vector3 dir = up * moveInput.y + right * moveInput.x;
        if (dir != Vector3.zero)
        {
            rotate = Quaternion.LookRotation(dir, Vector3.up);
        }
        else
        {
            rotate = Quaternion.identity;
        }
        return dir;
    }

    /// <summary>
    /// 想要得到目标方向，应该输入什么
    /// </summary>
    /// <param name="targetDirection"></param>
    /// <param name="camera"></param>
    /// <returns></returns>
    public Vector2 InverseTransformMoveDirection(Vector3 targetDirection, Camera camera)
    {
        Vector3 up = Vector3.ProjectOnPlane(camera.transform.up, Vector3.up).normalized;
        Vector3 right = Vector3.ProjectOnPlane(camera.transform.right, Vector3.up).normalized;
        targetDirection = Vector3.ProjectOnPlane(targetDirection, Vector3.up);
        float y = Vector3.Dot(up, Vector3.Project(targetDirection, up));
        float x = Vector3.Dot(right, Vector3.Project(targetDirection, right));
        return new Vector2(x, y);
    }
    #endregion
}
