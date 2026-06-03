using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/*
 memo:
 キーボード/パッドによる選択移動(Index)と、
 ScrollRectによるスクロール位置(ScrollRect座標)を同期するため、
 Index <-> ScrollRect座標(0～1)の相互変換を行っている

 例:
 ステージ数10、表示数5の場合
 ・選択移動側
 Stage0 ～ Stage9 をIndex(0 ～ 9)で管理
 ・スクロール移動側
 ScrollRect.verticalNormalizedPosition (1.0 ～ 0.0)で管理

 そのため
 Index → 表示割合 → Scroll座標
 Scroll座標 → 表示割合 → Index
 の変換処理が必要になる

 スクロール制御用のAsset/OSS導入も検討したが、
 現状はステージ数・機能ともに小規模で、追加コストを考慮して自前実装としている
 機能追加やスクロール仕様複雑化時は再検討する
 */

// FIXME:
// StageSelectボタン → Backボタン → StageSelectボタンへ戻った際、
// 選択位置が最下段へ移動する。進行不能ではないため後対応。
public class StageSelectController : MonoBehaviour
{
    [SerializeField]
    private int visibleStageCount = 5;

    [Header("Stage")]
    [SerializeField]
    private StageDataCatalog stageDataCatalog;
    [SerializeField]
    private Button stageButtonPrefab;
    [SerializeField]
    private Transform buttonParent;

    [Header("Scroll")]
    [SerializeField]
    private ScrollRect scrollRect;
    [SerializeField]
    private Button upButton;
    [SerializeField]
    private Button downButton;

    private readonly List<Button> stageButtons = new();

    private int topVisibleIndex;

    private int MaxScrollIndex => Mathf.Max(0, stageDataCatalog.Stages.Count - visibleStageCount);

    private GameObject lastSelectedStageButton;

    public void Initialize()
    {
        if (stageButtons.Count > 0) return;

        CreateButtons();

        upButton.onClick.AddListener(ScrollUpOneStep);
        downButton.onClick.AddListener(ScrollDownOneStep);
        scrollRect.onValueChanged.AddListener(_ => SyncIndexFromScroll());

        SetScrollIndex(0);
    }

    public GameObject GetFirstButton()
    {
        if (stageButtons.Count == 0) return null;

        return stageButtons[0].gameObject;
    }

    private void CreateButtons()
    {
        for (int i = 0; i < stageDataCatalog.Stages.Count; i++)
        {
            CreateButton(i, stageDataCatalog.Stages[i]);
        }
    }

    private void CreateButton(int index, StageData stage)
    {
        Button button = Instantiate(stageButtonPrefab, buttonParent);

        TMP_Text stageName = button.GetComponentInChildren<TMP_Text>();
        stageName.text = stage.DisplayName;

        StageSelectButton selectButton = button.GetComponent<StageSelectButton>();
        selectButton.Initialize(index, AdjustScrollToSelectedIndex, stageName, stage.IsPlayable);

        TitleMenuButton titleMenuButton = button.GetComponent<TitleMenuButton>();
        titleMenuButton.SetButtonType(stage.IsPlayable ? UIButtonType.Positive : UIButtonType.Locked);

        if (stage.IsPlayable)
        {
            button.onClick.AddListener(() => LoadStage(stage.SceneName));
        }

        stageButtons.Add(button);
    }

    private void LoadStage(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    private void ScrollUpOneStep()
    {
        SetScrollIndex(topVisibleIndex - 1);
    }

    private void ScrollDownOneStep()
    {
        SetScrollIndex(topVisibleIndex + 1);
    }

    /// <summary>
    /// スクロール位置を更新する
    /// 範囲外の値を補正し、ScrollRectと矢印状態も同期する
    /// </summary>
    /// <param name="index">(表示中先頭のステージのIndex</param>
    private void SetScrollIndex(int index)
    {
        // 0 ～ (ステージ数 - 表示可能数)の数に丸める
        topVisibleIndex = Mathf.Clamp(index, 0, MaxScrollIndex);

        ApplyScrollPosition();
        UpdateArrowButtons();
    }

    /// <summary>
    /// 現在のスクロールIndexをScrollRectへ反映する
    /// パッドやWASDの入力反映用
    /// </summary>
    private void ApplyScrollPosition()
    {
        scrollRect.verticalNormalizedPosition = ToScrollPosition(topVisibleIndex);
    }

    /// <summary>
    /// ScrollRectの現在位置からスクロールIndexを更新する
    /// マウスホイールやドラッグ操作時の同期用
    /// </summary>
    private void SyncIndexFromScroll()
    {
        int newIndex = ToScrollIndex(scrollRect.verticalNormalizedPosition);

        if (newIndex != topVisibleIndex)
        {
            topVisibleIndex = newIndex;
            UpdateArrowButtons();
        }
    }

    /// <summary>
    /// スクロールIndexをScrollRect用の正規化座標へ変換する
    /// </summary>
    /// <param name="index">表示中先頭のステージIndex</param>
    /// <returns>ScrollRectのverticalNormalizedPosition</returns>
    private float ToScrollPosition(int index)
    {
    if (MaxScrollIndex <= 0) return 1f;

    return 1f - (float)index / MaxScrollIndex;
    }

    /// <summary>
    /// ScrollRectの正規化座標をスクロールIndexへ変換する
    /// </summary>
    /// <param name="position">ScrollRectのverticalNormalizedPosition(表示中先頭Indexに対応する割合)</param>
    /// <returns>表示中先頭となるステージIndex</returns>
    private int ToScrollIndex(float position)
    {
        if (MaxScrollIndex <= 0) return 0;

        float scrollRate = 1f - position;

        return Mathf.RoundToInt(scrollRate * MaxScrollIndex);
    }

    /// <summary>
    /// 現在のスクロール位置に応じて矢印ボタンの有効状態を更新する
    /// </summary>
    private void UpdateArrowButtons()
    {
        upButton.interactable = topVisibleIndex > 0;
        downButton.interactable = topVisibleIndex < MaxScrollIndex;
    }

    public void RestoreLastSelectedStageButton()
    {
        if (lastSelectedStageButton == null)
        {
            lastSelectedStageButton =
                stageButtons[0].gameObject;
        }

        EventSystem.current.SetSelectedGameObject(null);

        EventSystem.current.SetSelectedGameObject(
            lastSelectedStageButton);
    }

    /// <summary>
    /// キーボード/パッド操作で端のステージを選択した場合
    /// 次の項目が見える位置までスクロール位置を調整する
    /// </summary>
    /// <param name="selectedIndex">選択されたステージIndex</param>
    private void AdjustScrollToSelectedIndex(int selectedIndex)
    {
        lastSelectedStageButton = stageButtons[selectedIndex].gameObject;

        int visibleBottomIndex = topVisibleIndex + visibleStageCount - 1;

        // 上端/下端を選択した場合1個上にスクロールして次の項目表示
        if (selectedIndex <= topVisibleIndex)
        {
            SetScrollIndex(selectedIndex - 1);
            return;
        }

        if (selectedIndex >= visibleBottomIndex)
        {
            SetScrollIndex(selectedIndex - visibleStageCount + 2);
        }
    }
}