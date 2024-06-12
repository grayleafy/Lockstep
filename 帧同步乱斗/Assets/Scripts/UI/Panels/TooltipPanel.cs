using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TooltipPanel : BasePanel
{
    [SerializeField]
    private TextMeshProUGUI title;
    [SerializeField]
    private TextMeshProUGUI content;
    [SerializeField]
    public Button okButton;
    [SerializeField]
    private RectTransform screenBlockout;

    [Header("是否屏蔽其它UI")]
    [SerializeField]
    private bool blockout = true;
    public bool Blockout
    {
        get => blockout;
        set
        {
            blockout = value;
            UpdateBlockout();
        }
    }

    public override void OnShow()
    {
        base.OnShow();
        UpdateBlockout();

        okButton.onClick.AddListener(() => Hide());
    }

    public override void OnStartHide()
    {
        base.OnStartHide();
        okButton.onClick.RemoveAllListeners();
    }

    /// <summary>
    /// 设置显示内容
    /// </summary>
    /// <param name="title"></param>
    /// <param name="content"></param>
    public void SetTooltip(string title, string content, bool blockout = true)
    {
        this.title.text = title;
        this.content.text = content;
        Blockout = blockout;
    }

    void UpdateBlockout()
    {
        screenBlockout.gameObject.SetActive(blockout);
    }
}
