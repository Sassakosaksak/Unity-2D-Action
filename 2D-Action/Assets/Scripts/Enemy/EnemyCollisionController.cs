using UnityEngine;

public class EnemyCollisionController : MonoBehaviour
{
    private Collider2D enemyBodyCollider;

    private void Start()
    {
        enemyBodyCollider = GetComponentInParent<Rigidbody2D>().GetComponent<Collider2D>();
        
        PlayerController player = FindFirstObjectByType<PlayerController>();
        if (player == null) return;
        if (player.BodyCollider == null) return;
        if (enemyBodyCollider == null) return;

        // 無敵時間中はすり抜け可能になるように
        // プレイヤーと敵の基本的な衝突判定を無視する処理
        Physics2D.IgnoreCollision(player.BodyCollider, enemyBodyCollider, true);
    }
}