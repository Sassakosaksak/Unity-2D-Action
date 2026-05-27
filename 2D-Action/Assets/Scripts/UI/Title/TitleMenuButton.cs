using UnityEngine;
using UnityEngine.EventSystems;

public class TitleMenuButton :
    MonoBehaviour,
    IPointerEnterHandler,
    IPointerClickHandler,
    ISelectHandler,
    ISubmitHandler,
    IDeselectHandler
{
    [SerializeField]
    private UIButtonType buttonType;

    private bool selected;

    public static GameObject CurrentHoveredObject { get; private set; }
    public static GameObject LastSelectedObject { get; private set; }

    public void SetButtonType(UIButtonType buttonType)
    {
        this.buttonType = buttonType;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        CurrentHoveredObject = gameObject;

        EventSystem.current.SetSelectedGameObject(null);

        selected = false;

        UISEManager.Instance.PlayHover();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (CurrentHoveredObject == gameObject)
        {
            CurrentHoveredObject = null;
        }
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (selected) return;

        selected = true;
        LastSelectedObject = gameObject;

        UISEManager.Instance.PlayHover();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        PlaySubmit();
    }

    public void OnSubmit(BaseEventData eventData)
    {
        PlaySubmit();
    }

    private void OnDisable()
    {
        selected = false;

        if (CurrentHoveredObject == gameObject)
        {
            CurrentHoveredObject = null;
        }
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

            case UIButtonType.Locked:
                UISEManager.Instance.PlayLocked();
                break;

            default:
                Debug.LogWarning($"Unsupported button type: {buttonType}", this);
                UISEManager.Instance.PlayPositive();
                break;
        }
    }
}