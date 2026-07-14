using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class MinotaurAxeHitBox : MonoBehaviour
{
    [SerializeField]
    private int firstAttackPower = 1;
    [SerializeField]
    private int secondAttackPower = 2;

    private Enemy_Minotaur minotaur;

    private void Awake()
    {
        minotaur = GetComponentInParent<Enemy_Minotaur>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("HurtBox")) return;

        PlayerController player = other.GetComponentInParent<PlayerController>();
        if (player == null) return;

        int attackPower =
            minotaur.IsSecondAttack ? secondAttackPower : firstAttackPower;

        player.TakeDamage(attackPower, transform.position);
    }
}
