using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 只有高亮的button
/// </summary>
public class PicoButton : Button
{
    protected override void Awake()
    {
        base.Awake();
        onClick.AddListener(() => { DoStateTransition(SelectionState.Highlighted, false); });
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        if (interactable)
        {
            DoStateTransition(SelectionState.Highlighted, true);
        }
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
        if (interactable)
        {
            DoStateTransition(SelectionState.Normal, true);
        }
    }
}
