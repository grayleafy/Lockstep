using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;


public enum UIType
{
    Static,
    Dynamic,
}
public class BasePanel : MonoBehaviour
{
    public UILayer uILayer = UILayer.Mid;
    public bool destroyOnHide = true;
    public bool uniquePanel = true;
    public float hideDelay = 0;
    public UIType type = UIType.Static;

    /// <summary>
    /// 加载完成，开始显示
    /// </summary>
    public virtual void OnShow()
    {

    }

    public void Hide()
    {
        OnStartHide();
        Invoke("OnEndHide", hideDelay);
    }

    /// <summary>
    /// 开始关闭，还未销毁
    /// </summary>
    public virtual void OnStartHide()
    {

    }

    /// <summary>
    /// 已经销毁后的事件
    /// </summary>
    public virtual void OnEndHide()
    {
        if (uniquePanel)
        {
            UIMgr.Instance.RemoveUniquePanel(gameObject.name);
        }


        if (destroyOnHide == false)
        {
            PoolMgr.Instance?.PushObj(UIMgr.Instance.uiAbName, gameObject.name, gameObject);
        }
        else
        {
            GameObject.Destroy(gameObject);
        }
    }



}
