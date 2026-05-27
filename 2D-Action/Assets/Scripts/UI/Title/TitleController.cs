using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class TitleController : MonoBehaviour
{
    [Serializable]
    private class TitleLayerEntry
    {
        public TitleLayer layer;
        public GameObject layerObject;
        public GameObject firstSelectedButton;
    }

    [Header("Layer")]
    [SerializeField]
    private TitleLayerEntry[] layerEntries;
    [SerializeField]
    private GameObject titleLogo;

    [Header("Controller")]
    [SerializeField]
    private StageSelectController stageSelectController;
    
    private TitleLayer currentLayer;

    [SerializeField]
    private InputActionReference navigateAction;

    private void Start()
    {
        ShowLayer(TitleLayer.Top);
    }
    private void Update()
    {
        UpdateMouseCursorVisible();
    }

    // FIXME：キー/マウス切り替え対応のため暫定実装
    // 入力方式の判定・選択状態復元・カーソル表示制御が混在しているため、本実装のタイミングで要リファクタ。

    // FIXME：ホバー状態のままキー操作した場合、ホバー色が残る。
    // 進行不能でないため、本実装のタイミングで要リファクタ
    #region キー/マウス操作切り替え用
    private void OnEnable()
    {
        navigateAction.action.performed += OnNavigate;
    }

    private void OnDisable()
    {
        navigateAction.action.performed -= OnNavigate;
    }

    private void OnNavigate(InputAction.CallbackContext context)
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = false;

        RestoreLastSelectedObject();
    }

    private void RestoreLastSelectedObject()
    {
        if (EventSystem.current.currentSelectedGameObject != null) return;
        if (TitleMenuButton.LastSelectedObject == null) return;

        EventSystem.current.SetSelectedGameObject(TitleMenuButton.LastSelectedObject);
    }

    private void UpdateMouseCursorVisible()
    {
        if (Mouse.current == null) return;

        if (Mouse.current.delta.ReadValue() == Vector2.zero) return;

        Cursor.visible = true;
    }

    #endregion

    public void PushStart()
    {
        ShowLayer(TitleLayer.StageSelect);
    }

    public void PushOption()
    {
        ShowLayer(TitleLayer.Option);
    }

    public void PushBack()
    {
        ShowLayer(TitleLayer.Top);
    }

    public void PushQuit()
    {
        Application.Quit();
    }

    private void ShowLayer(TitleLayer titleLayer)
    {
        HideAllLayers();

        TitleLayerEntry target = GetLayerEntry(titleLayer);

        if (target == null)
        {
            Debug.LogError($"Layer not found : {titleLayer}", this);
            return;
        }

        currentLayer = titleLayer;

        target.layerObject.SetActive(true);

        ApplyLayerSpecificSetup(titleLayer);

        GameObject firstSelectedButton = GetFirstSelectedButton(target);

        SelectButton(firstSelectedButton);
    }

    private void HideAllLayers()
    {
        foreach (TitleLayerEntry entry in layerEntries)
        {
            if (entry.layerObject == null) continue;

            entry.layerObject.SetActive(false);
        }
    }

    private TitleLayerEntry GetLayerEntry(TitleLayer titleLayer)
    {
        foreach (TitleLayerEntry entry in layerEntries)
        {
            if (entry.layer == titleLayer)
            {
                return entry;
            }
        }

        return null;
    }

    private void ApplyLayerSpecificSetup(TitleLayer titleLayer)
    {
        // NOTE：レイヤー数が少なく、差分処理もタイトル専用のため個別に処理
        // 差分が増えたら、各LayerControllerへの分離を検討
        if (titleLogo != null)
        {
            titleLogo.SetActive(titleLayer != TitleLayer.Option);
        }

        if (titleLayer == TitleLayer.StageSelect)
        {
            stageSelectController.Initialize();
        }
    }

    private GameObject GetFirstSelectedButton(TitleLayerEntry entry)
    {
        // StageSelectはOpen時にボタン生成するため、生成後の先頭を選択
        if (entry.layer == TitleLayer.StageSelect)
        {
            return stageSelectController.GetFirstButton();
        }

        return entry.firstSelectedButton;
    }

    private void SelectButton(GameObject target)
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(target);
    }
}

public enum TitleLayer
{
    Top,
    StageSelect,
    Option
}