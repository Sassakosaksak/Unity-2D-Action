using System.Collections;
using UnityEngine;

public class PlayerDamageController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator animator;
    private AnimationEffectController animEffect;
    private Collider2D bodyCollider;
    private PlayerController playerController;

    [Header("HP")]
    [SerializeField]
    private int maxHP = 10;
    [SerializeField]
    private int currentHP;
    [SerializeField]
    private HPBar hpBar;

    [Header("Damage")]
    [SerializeField]
    private float invincibleTime = 1f;
    [SerializeField]
    private float knockBackXPower = 2f;
    [SerializeField]
    private float knockBackYPower = 3f;
    [SerializeField]
    private float knockBackControlLockTime = 0.25f;

    private bool isInvincible;
    private bool isDead;
    private bool isHit;
    private bool isKnockBacking;

    public bool IsDead => isDead;
    public bool IsHit => isHit;
    public bool IsKnockBacking => isKnockBacking;
    public Collider2D BodyCollider => bodyCollider;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animEffect = GetComponent<AnimationEffectController>();
        bodyCollider = GetComponent<Collider2D>();
        playerController = GetComponent<PlayerController>();
        animator = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        currentHP = maxHP;
        hpBar.SetHP(currentHP, maxHP);
    }

    public void TakeDamage(int damage, Vector2 attackerPosition)
    {
        if (isInvincible) return;
        if (isDead) return;

        playerController.CancelAttack();

        currentHP -= damage;
        hpBar.SetHP(currentHP, maxHP);

        if (currentHP <= 0)
        {
            Die();
            return;
        }

        StartCoroutine(DamageSequence(attackerPosition));
    }

    private IEnumerator DamageSequence(Vector2 attackerPosition)
    {
        isInvincible = true;
        isKnockBacking = true;
        isHit = true;

        animator.SetBool("IsHit", isHit);

        animEffect.PlayHitPunch();
        animEffect.PlayInvincibleBlink();

        float dirX = transform.position.x >= attackerPosition.x ? 1f : -1f;

        rb.linearVelocity = Vector2.zero;
        rb.linearVelocity = new Vector2(dirX * knockBackXPower, knockBackYPower);

        yield return new WaitForSeconds(knockBackControlLockTime);

        RecoverFromHit();

        yield return new WaitForSeconds(invincibleTime - knockBackControlLockTime);

        isInvincible = false;
        animEffect.StopInvincibleBlink();
    }

    public void RecoverFromHit()
    {
        isKnockBacking = false;
        isHit = false;
        animator.SetBool("IsHit", isHit);
    }

    private void Die()
    {
        isDead = true;
        gameObject.layer = LayerMask.NameToLayer("DeadPlayer");
        rb.linearVelocity = Vector2.zero;

        animator.SetTrigger("Die");
        animator.SetBool("IsDead", true);

        StartCoroutine(DieSequence());
    }

    private IEnumerator DieSequence()
    {
        Time.timeScale = 0f;

        yield return new WaitForSecondsRealtime(0.5f);

        Time.timeScale = 0.2f;

        yield return new WaitForSecondsRealtime(1f);

        Time.timeScale = 1f;

        yield return new WaitForSeconds(1.5f);

        GameManager.Instance.GameOver();
    }
}