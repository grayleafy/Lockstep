using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuPanel : BasePanel
{
    public DelayMove buttons;
    public float animOffset = 1000;
    public float animDuration = 1f;

    [Header("按钮")]
    public Button enterButton;
    public Button settingButton;
    public Button exitButton;

    public override void OnShow()
    {
        base.OnShow();
        //动画
        buttons.ResetChildren();
        buttons.SetOffset(new Vector3(0, -animOffset, 0));
        buttons.Move(new Vector3(0, animOffset, 0), animDuration, true);

        //按键监听
        enterButton.onClick.AddListener(() =>
        {
            Hide();
            TimeMgr.Instance.DelayCallForRealTime(animDuration, () => UIMgr.Instance.ShowPanel<LoginPanel>("LoginPanel"));
        });
        settingButton.onClick.AddListener(() =>
        {
            Debug.Log("打开设置界面");
        });
        exitButton.onClick.AddListener(() =>
        {
            Application.Quit();
        });
    }

    public override void OnStartHide()
    {
        base.OnStartHide();
        //动画
        buttons.Move(new Vector3(0, -animOffset, 0), animDuration, false);

        //按键
        enterButton.onClick.RemoveAllListeners();
        settingButton.onClick.RemoveAllListeners();
        exitButton.onClick.RemoveAllListeners();
    }
}
