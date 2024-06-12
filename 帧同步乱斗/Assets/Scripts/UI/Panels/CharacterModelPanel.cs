using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterModelPanel : BasePanel, IView
{
    [SerializeField]
    CharacterInfo characterInfo;
    IModel IView.Data { get => characterInfo; set => characterInfo = value as CharacterInfo; }

    public RawImage rawImage;


    public void RefreshView(IModel data)
    {
        rawImage.texture = ModelRenderMgr.Instance.GetRenderTexture(characterInfo.fullName);
    }
}
