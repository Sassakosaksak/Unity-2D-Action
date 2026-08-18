using UnityEngine;

public class PlayerWeaponHitBox : MonoBehaviour
{
    [SerializeField]
    private Collider2D hitBoxCollider;
    private PlayerAttackController playerAttackController;

    // ïêäÌÇ≈ïœÇ¶ÇÁÇÍÇÈÇÊÇ§Ç…
    protected int attackPower = 2;

    private void Awake()
    {
        playerAttackController = GetComponentInParent<PlayerAttackController>();
    }

    private void Start()
    {
        hitBoxCollider.enabled = false;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out EnemyControllerBase enemy))
        {
            enemy.TakeDamage(attackPower, playerAttackController.AttackDirectionX);
            return;
        }

        if (other.TryGetComponent(out IBreakable breakable))
        {
            breakable.Break();
            return;
        }
    }

    public void EnableAttack()
    {
        hitBoxCollider.enabled = true;
    }

    public void DisableAttack()
    {
        hitBoxCollider.enabled = false;
    }

    private void OnDrawGizmos()
    {
        if (hitBoxCollider == null) return;

        Gizmos.color = new Color(1f, 0f, 0f, 0.3f);
        Gizmos.DrawCube(hitBoxCollider.bounds.center, hitBoxCollider.bounds.size);

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(hitBoxCollider.bounds.center, hitBoxCollider.bounds.size);
    }
}