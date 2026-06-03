using System.Collections;
using UnityEngine;

/*
    チュートリアルステージの流れ
    ・画面外からプレイヤー登場（カットシーン）
    ・探索
    ・宝箱を開ける（ここで敵出現+初期位置にゴール出現）
    ・敵と戦闘（しなくてもよい）
    ・初期位置に戻ってゴール
 */

public class TutorialStageFlow : StageFlowBase
{

    [SerializeField]
    private GameObject player;
    [SerializeField]
    private Chest chest;
    [SerializeField]
    private GameObject enemyGroup;
    [SerializeField]
    private GameObject goal;
    [SerializeField]
    private GameObject playerMessageGroup;

    private PlayerController playerController;
    private Rigidbody2D playerRb;
    private Animator playerAnimator;
    private PlayerMessageUI playerMessageUI;

    protected override void OnStageStart()
    {
        playerController = player.GetComponent<PlayerController>();
        playerRb = player.GetComponent<Rigidbody2D>();
        playerAnimator = player.GetComponentInChildren<Animator>();
        playerMessageUI = playerMessageGroup.GetComponentInChildren<PlayerMessageUI>();

        StartCoroutine(StartSequence());
    }

    [ContextMenu("ChestOpen")]
    private void DebugChestOpen()
    {
        OnChestOpened();
    }

    private void OnEnable()
    {
        // チェスト開いたタイミングで、敵、ゴール出現の処理が呼ばれるように登録
        chest.Opened += OnChestOpened;
    }

    private void OnDisable()
    {
        chest.Opened -= OnChestOpened;
    }

    private IEnumerator StartSequence()
    {
        // 操作OFF
        playerController.SetInputEnabled(false);
        playerController.AutoMove(-0.4f);
        yield return new WaitForSeconds(2f);
        playerController.AutoMoveStop();

        // TODO:お宝見つけるぞの意気込みエモートを挿入。進行に支障ないのでエモート素材の準備後に対応

        yield return new WaitForSeconds(1f);

        // 操作ON
        playerController.SetInputEnabled(true);
    }

    public void OnChestOpened()
    {
        enemyGroup.SetActive(true);
        goal.SetActive(true);

        StartCoroutine(ChestOpenedSequence());
    }

    protected override void OnStageClear()
    {
        StartCoroutine(ClearSequence());
    }

    private IEnumerator ChestOpenedSequence()
    {
        playerController.SetInputEnabled(false);

        yield return new WaitForSeconds(2f);

        chest.PlayEmptySE();
        playerMessageUI.ShowMessage("Empty......");

        playerController.SetInputEnabled(true);
    }

    private IEnumerator ClearSequence()
    {
        // ゴールを非表示
        goal.SetActive(false);

        // 操作OFF
        playerController.SetInputEnabled(false);

        // 右へ走り抜ける
        playerController.AutoMove(1f);
        yield return new WaitForSeconds(2f);
        playerController.AutoMoveStop();

        // クリアUI表示
        GameManager.Instance.GameClear();
    }

}