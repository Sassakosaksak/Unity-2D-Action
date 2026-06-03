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
    private UISEController uiSE;

    [SerializeField]
    private UIButtonType buttonType;

    private bool selected;

    public static GameObject CurrentHoveredObject { get; private set; }
    public static GameObject LastSelectedObject { get; private set; }

    private void Awake()
    {
        uiSE = GetComponent<UISEController>();
    }

    public void SetButtonType(UIButtonType buttonType)
    {
        this.buttonType = buttonType;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (CurrentHoveredObject == gameObject) return;

        CurrentHoveredObject = gameObject;

        uiSE.PlayHover();
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

        uiSE.PlayHover();
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
                uiSE.PlayPositive();
                break;

            case UIButtonType.Negative:
                uiSE.PlayNegative();
                break;

            case UIButtonType.Locked:
                uiSE.PlayLocked();
                break;

            default:
                Debug.LogWarning($"Unsupported button type: {buttonType}", this);
                uiSE.PlayPositive();
                break;
        }
    }
}