using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class StageSelectButton : MonoBehaviour, ISelectHandler
{
    [SerializeField]
    private Color playableTextColor = Color.white;
    [SerializeField]
    private Color unplayableTextColor = new(0.6f, 0.6f, 0.6f); // グレー

    private int index;
    private Action<int> onSelected;
    private TMP_Text label;
    private bool isPlayable;
    private bool selectedByPointer;

    public void Initialize(
        int index,
        Action<int> onSelected,
        TMP_Text label,
        bool isPlayable)
    {
        this.index = index;
        this.onSelected = onSelected;
        this.label = label;
        this.isPlayable = isPlayable;

        ApplyVisual();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        /// マウスカーソルの場合は自動スクロール無効にする
        selectedByPointer = true;
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (eventData is PointerEventData) return;

        onSelected?.Invoke(index);
    }

    private void ApplyVisual()
    {
        if (label == null) return;

        label.color = isPlayable ? playableTextColor : unplayableTextColor;
    }
}