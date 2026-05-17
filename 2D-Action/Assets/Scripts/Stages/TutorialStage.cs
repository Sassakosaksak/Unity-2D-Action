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

    private PlayerController playerController;
    private Rigidbody2D playerRb;
    private Animator playerAnimator;

    protected override void OnStageStart()
    {
        playerController = player.GetComponent<PlayerController>();
        playerRb = player.GetComponent<Rigidbody2D>();
        playerAnimator = player.GetComponentInChildren<Animator>();

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
        playerController.enabled = false;

        // 右から左に移動
        //playerAnimator.SetTrigger // Speedでanimator勝手にRunになるはず、ならなかったらTriggerとかつける
        playerController.Move(-0.4f);
        //TODO:トリガーとかで動きとめる形に変更
        yield return new WaitForSeconds(2f);
        playerController.Stop();

        // TODO:お宝見つけるぞの意気込みエモートを挿入

        yield return new WaitForSeconds(1f);

        // 操作ON
        playerController.enabled = true;
    }

    public void OnChestOpened()
    {
        enemyGroup.SetActive(true);
        goal.SetActive(true);
    }

    protected override void OnStageClear()
    {
        StartCoroutine(ClearSequence());
    }

    private IEnumerator ClearSequence()
    {
        // ゴールを非表示
        goal.SetActive(false);

        // 操作OFF
        playerController.enabled = false;

        // 右へ走り抜ける
        playerController.Move(1f);
        yield return new WaitForSeconds(2f);
        playerController.Stop();

        // クリアUI表示
        GameManager.Instance.GameClear();
    }

}