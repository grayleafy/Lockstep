using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TapPanel : BasePanel
{
    public Transform tapLabel;
    public float fadeTime = 0.5f;
    public TextFlash textFlash;

    private bool input = true;

    private void Update()
    {
        if (input && Input.anyKeyDown)
        {
            input = false;
            TimeMgr.Instance.DelayCallForRealTime(0.5f, () => UIMgr.Instance.ShowPanel<MainMenuPanel>("MainMenuPanel"));
            Hide();
            GameMain.Instance.Init();
        }
    }

    public override void OnShow()
    {
        base.OnShow();
        input = true;
        textFlash.enabled = true;
    }

    public override void OnStartHide()
    {
        base.OnStartHide();

        textFlash.enabled = false;

        var images = tapLabel.GetComponentsInChildren<Image>();
        foreach (var image in images)
        {
            image.DOFade(0, fadeTime);
        }

        var texts = tapLabel.GetComponentsInChildren<TextMeshProUGUI>();
        foreach (var text in texts)
        {
            text.DOFade(0, fadeTime);
        }
    }
}
