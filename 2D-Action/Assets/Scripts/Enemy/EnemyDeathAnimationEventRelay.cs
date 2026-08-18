using UnityEngine;

public class EnemyDeathAnimationEventRelay : MonoBehaviour
{
    private EnemyControllerBase enemyController;

    private void Awake()
    {
        enemyController = GetComponentInParent<EnemyControllerBase>();
    }

    /// <summary>
    /// 死亡アニメーション終了時の処理を敵本体に伝える
    /// </summary>
    public void Anim_DieEnd()
    {
        if (enemyController == null) return;

        enemyController.Anim_DieEnd();
    }
}
