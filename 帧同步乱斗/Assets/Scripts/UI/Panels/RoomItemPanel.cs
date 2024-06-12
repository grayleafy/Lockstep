using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RoomItemPanel : MonoBehaviour, IView
{
    public TextMeshProUGUI roomName;
    public TextMeshProUGUI sceneName;
    public TextMeshProUGUI memberCount;
    public Button button;
    public RectTransform selectHighLight;


    private Room room;

    IModel IView.Data { get => room; set => room = value as Room; }




    private void OnDestroy()
    {
        (this as IView).UnbindData();
    }


    public void RefreshView(IModel data)
    {
        if (room == null) return;
        roomName.text = room.Name;
        sceneName.text = LevelMgr.Instance.GetLevel(room.LevelId).name;
        memberCount.text = room.CurrentMemberCount.ToString() + "<size=32><#beb5b6>/" + room.MaxMember.ToString();
    }

    public void OnSelect()
    {
        selectHighLight.gameObject.SetActive(true);
    }

    public void OnDeselect()
    {
        selectHighLight.gameObject.SetActive(true);
    }
}
