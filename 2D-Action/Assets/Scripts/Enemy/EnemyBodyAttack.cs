using UnityEngine;

public class EnemyBodyAttack : MonoBehaviour
{
    private EnemyControllerBase enemy;

    void Start()
    {
        enemy = GetComponentInParent<EnemyControllerBase>();
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
