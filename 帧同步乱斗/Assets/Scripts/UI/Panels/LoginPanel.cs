using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class LoginPanel : BasePanel
{
    public Button closeButton;
    public Button okButton;
    public TextMeshProUGUI connectingText;
    public TMP_InputField playerNameInput;
    public TextMeshProUGUI toolTip; //登录结果显示

    public DelayMove delayMove;
    public float animDuration = 0.4f;
    public Vector3 animOffset = new Vector3(0, -1000, 0);

    private void Reset()
    {
        hideDelay = 1f;
    }

    public override void OnShow()
    {
        base.OnShow();
        //动画
        delayMove.MoveFromOffset(animOffset, animDuration, true);

        //按钮
        closeButton.onClick.AddListener(() =>
        {
            Hide();
            TimeMgr.Instance.DelayCallForRealTime(animDuration, () => UIMgr.Instance.ShowPanel<MainMenuPanel>("MainMenuPanel"));
        });
        okButton.onClick.AddListener(BeginLogin);

        //重制按钮
        ResetButton();
    }


    public override void OnStartHide()
    {
        base.OnStartHide();
        //动画
        delayMove.Move(animOffset, animDuration, false);

        //按钮
        closeButton.onClick.RemoveAllListeners();
        okButton.onClick.RemoveAllListeners();
    }

    void BeginLogin()
    {
        toolTip.text = "";

        //本地检测用户名合法性
        if (playerNameInput.text == "" || playerNameInput.text == null)
        {
            toolTip.text = "用户名不能为空";
        }
        else
        {
            PlayerMgr.Instance.BeginLogin(playerNameInput.text, EndLogin);
            HideOkButton();
        }

    }

    //登录完成
    void EndLogin(LoginResMsg msg)
    {

        //登录成功
        if (msg.state == LoginState.Success)
        {
            if (msg.playState == GameState.Normal)
            {
                Hide();
                //打开选房界面
                UIMgr.Instance.ShowPanel<JoinOrCreateRoomPanel>("JoinOrCreateRoomPanel");
            }

        }
        //失败
        else
        {
            ResetButton();
            toolTip.text = msg.state.ToString();
        }
    }

    //隐藏登录按钮
    void HideOkButton()
    {
        okButton.gameObject.SetActive(false);
        connectingText.gameObject.SetActive(true);
    }

    //重置登录按钮
    void ResetButton()
    {
        okButton.gameObject.SetActive(true);
        connectingText.gameObject.SetActive(false);
    }
}
