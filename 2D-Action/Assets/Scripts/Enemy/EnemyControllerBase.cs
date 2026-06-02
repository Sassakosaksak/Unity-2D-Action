using System.Collections;
using UnityEngine;

public abstract class EnemyControllerBase : MonoBehaviour
{
    [Header("Reference")]
    protected Transform player;
    protected Rigidbody2D rb;
    protected Animator animator;
    protected AnimationEffectController animEffect;
    [SerializeField]
    [Tooltip("各エネミーのSEControllerを設定")]
    protected EnemyBaseSEController enemyBaseSEController;
    [SerializeField]
    protected Collider2D bodyAttackCol;

    [Header("Status")]
    public int maxHP = 10;
    [SerializeField]
    protected int currentHP;
    [SerializeField]
    protected bool rightFacing = false;

    [Header("Combat")]
    [SerializeField]
    protected int bodyAttackDamage = 1;
    [SerializeField]
    private float knockBackPower = 3f;
    [SerializeField]
    private float invincibleTime = 0.3f;
    [SerializeField]
    protected bool isKnockBacking = false;
    [SerializeField]
    private float knockBackTime = 0.5f;
    [SerializeField]
    private float knockBackDecay = 0.9f;

    protected bool isDead = false;
    protected bool isInvincible = false;

    [Header("Collision of Between Enemies")]
    [SerializeField]
    private float separationRadius = 0.5f;
    [SerializeField]
    private float separationPower = 1.5f;
    [SerializeField]
    private LayerMask enemyLayer;

    // Note:ヒットストップの値は現状固定
    // 演出や攻撃に応じて変更するタイミングで入力側から入れられるように調整
    private float hitStopDuration = 0.13f;
    private float hitStopDurationOfDie = 0.2f;
    private float hitStopScale = 0.05f;

    /// <summary>
    /// キャラクターのレベル
    /// 速度やダメージなどに影響を与える予定
    /// 仮置きで1f
    /// </summary>
    [SerializeField]
    protected float Level = 1f;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        animEffect = GetComponentInChildren<AnimationEffectController>();
    }

    protected virtual void Start()
    {
        currentHP = maxHP;

        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }

        ApplyFacing();
    }

    protected virtual void Update()
    {
        if (!CanAct()) return;
    }

    protected virtual void FixedUpdate()
    {
        if (isKnockBacking)
        {
            rb.linearVelocity *= knockBackDecay;
            return;
        }
    }

    public virtual void TakeDamage(int damage, Vector2 attackerPosition)
    {
        if (!CanTakeDamage()) return;

        enemyBaseSEController.PlayHit();
        ApplyDamage(damage);
        KnockBack(attackerPosition);

        if (currentHP <= 0)
        {
            HitStopManager.Instance.Play(hitStopDurationOfDie, hitStopScale);
            Die();
            return;
        }
        HitStopManager.Instance.Play(hitStopDuration, hitStopScale);
        Hit();

    }

    protected virtual void Hit()
    {
        if (animator != null)
        {
            animator.SetTrigger("Hit");
            animEffect.PlayHitFlash();
            animEffect.PlayHitPunch();

            StartCoroutine(InvincibleCoroutine());
        }
    }

    protected virtual void Die()
    {
        isDead = true;
        bodyAttackCol.enabled = false;

        enemyBaseSEController.PlayDie();

        if (animator != null)
        {
            rb.linearVelocity = Vector2.zero;
            animator.SetBool("IsDie", true);
            animEffect.PlayHitFlash();
            animEffect.PlayHitPunch();
            animEffect.PlayHitShake();
        }

        // TODO:Dieアニメーション後にDestroyするように修正
        Destroy(gameObject, 1.5f);
    }

    protected virtual void KnockBack(Vector2 attackerPosition)
    {
        StartCoroutine(KnockBackCoroutine(attackerPosition));
    }

    private IEnumerator KnockBackCoroutine(Vector2 attackerPosition)
    {
        isKnockBacking = true;

        Vector2 direction =
            ((Vector2)transform.position - attackerPosition).normalized;

        rb.linearVelocity = new Vector2(direction.x * knockBackPower, rb.linearVelocity.y);
        // 自然な吹っ飛びにするための倍率
        yield return new WaitForSeconds(knockBackTime);

        rb.linearVelocity = Vector2.zero;
        isKnockBacking = false;

        RecoverFromHit();
    }

    protected virtual IEnumerator InvincibleCoroutine()
    {
        isInvincible = true;

        animEffect.PlayInvincibleBlink();

        yield return new WaitForSeconds(invincibleTime);

        animEffect.StopInvincibleBlink();

        isInvincible = false;
    }

    protected float GetDistanceToPlayer()
    {
        if (player == null) return Mathf.Infinity;
        return Vector2.Distance(transform.position, player.position);
    }

    public virtual void BodyAttack(PlayerController player)
    {
        if( player == null) return;

        rb.linearVelocity = Vector2.zero;
        player.TakeDamage(bodyAttackDamage, transform.position);
    }

    /// <summary>
    /// オブジェクトの反転
    /// </summary>
    /// <param name="faceRight">true:右向きに変更、false:左向きに変更。方向が一致している場合はSkip</param>
    protected virtual void Flip(bool faceRight)
    {
        if (rightFacing == faceRight) return;

        FaceTo(faceRight);
    }

    protected virtual void FlipToPlayer()
    {
        if (player == null) return;
        bool faceRight = player.position.x > transform.position.x;

        Flip(faceRight);
    }

    protected virtual void RecoverFromHit()
    {

    }

    protected virtual bool CanTakeDamage()
    {
        return !isDead && !isInvincible;
    }

    protected virtual bool CanAct()
    {
        return !isDead && !isKnockBacking;
    }

    protected virtual void ApplyDamage(int damage)
    {
        currentHP -= damage;
    }

    protected virtual void FaceTo(bool faceRight)
    {
        rightFacing = faceRight;
        ApplyFacing();
    }

    private void ApplyFacing()
    {
        // 敵オブジェクトはデフォルト左向きで作っているので
        // !rightFacingで(1,1,1)を設定
        if (!rightFacing)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }
    protected virtual bool IsPlayerDead()
    {
        if (player == null) return true;

        PlayerController playerController = player.GetComponent<PlayerController>();

        return playerController != null && playerController.IsDead;
    }

    /// <summary>
    /// 初期化時などの移動停止処理
    /// デフォルト1秒
    /// </summary>
    /// <param name="sec"></param>
    /// <returns></returns>
    protected IEnumerator Wait(float sec = 1f)
    {
        rb.linearVelocity = Vector2.zero;

        yield return new WaitForSeconds(sec);
    }

    protected Vector2 GetSeparationVelocity()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, separationRadius, enemyLayer);

        Vector2 force = Vector2.zero;

        foreach (var hit in hits)
        {
            if (hit.gameObject == gameObject) continue;

            Vector2 diff = (Vector2)(transform.position - hit.transform.position);
            float distance = diff.magnitude;

            // 0除算回避
            if (distance <= 0.01f) continue;

            force += diff.normalized / distance;
        }

        return force * separationPower;
    }
}