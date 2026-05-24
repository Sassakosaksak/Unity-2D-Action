using UnityEngine;

public class EnemyBodyAttack : MonoBehaviour
{
    private EnemyControllerBase enemy;

    void Start()
    {
        enemy = GetComponentInParent<EnemyControllerBase>();

        // TODO: 最終的に不要なら削除。ColliderのサイズはInspectorで各Prefabで設定しておく方が安全そう。
        //var colliderTouchArea = GetComponent<BoxCollider2D>();
        //var colliderBody = enemy.gameObject.GetComponent<BoxCollider2D>();
        //colliderTouchArea.offset = colliderBody.offset;
        //colliderTouchArea.size = colliderBody.size;
        //colliderTouchArea.size *= 0.8f;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!collision.CompareTag("HurtBox"))
        {
            return;
        }

        PlayerController player = collision.GetComponentInParent<PlayerController>();

        if (player == null)
        {
            return;
        }

        enemy.BodyAttack(player);
    }
}
