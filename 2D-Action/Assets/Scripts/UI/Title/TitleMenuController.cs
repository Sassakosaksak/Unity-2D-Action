using UnityEngine;
using UnityEngine.EventSystems;

public enum UIButtonType
{
    Positive,
    Negative
}

public class TitleMenuController :
    MonoBehaviour,
    IPointerEnterHandler,
    IPointerClickHandler,
    ISelectHandler,
    ISubmitHandler
{
    [SerializeField]
    private UIButtonType buttonType;

    private bool selected;

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("Pointer Enter", this); 
        UISEManager.Instance.PlayHover();
    }

    public void OnSelect(BaseEventData eventData)
    {
        Debug.Log("On Select", this);
        if (selected) return;

        selected = true;

        UISEManager.Instance.PlayHover();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Pointer Click", this);
        PlaySubmit();
    }

    public void OnSubmit(BaseEventData eventData)
    {
        Debug.Log("Submit", this);
        PlaySubmit();
    }

    private void OnDisable()
    {
        selected = false;
    }
    public void OnDeselect(BaseEventData eventData)
    {
        selected = false;
    }

    private void PlaySubmit()
    {
        switch (buttonType)
        {
            case UIButtonType.Positive:
                UISEManager.Instance.PlayPositive();
                break;

            case UIButtonType.Negative:
                UISEManager.Instance.PlayNegative();
                break;

            default:
                Debug.LogWarning($"Unsupported button type: {buttonType}", this);
                UISEManager.Instance.PlayPositive();
                break;
        }
    }
}