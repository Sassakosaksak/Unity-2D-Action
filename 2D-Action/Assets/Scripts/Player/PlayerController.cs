using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D rb;
    AnimationEffectController animEffect;
    Animator animator;
    [SerializeField]
    PlayerGroundSensor groundSensor;
    private Collider2D bodyCollider;
    public Collider2D BodyCollider => bodyCollider;

    Vector2 moveInput;

    [Header("ステータス関連")]
    [SerializeField]
    private int maxHP = 10;
    [SerializeField]
    private int currentHP;
    private bool isInvincible;
    private bool isDead;
    public bool IsDead => isDead;
    [SerializeField]
    private float invincibleTime = 1f;
    private bool isGrounded;
    private bool isAttacking;
    private bool isHit;
    [SerializeField]
    private bool rightFacing = true;
    private bool isKnockBacking = false;
    private bool isAutoMoving = false;
    private float autoMoveDirection = 1f;
    private bool canInput = true;

    private int comboStep = 0;
    private bool canComboInput = false;

    private int maxComboStep = 3;

    private State currentState;

    [SerializeField]
    private HPBar hpBar;

    [SerializeField]
    public float speed = 10f;
    [SerializeField]
    private float Accel = 20f;
    [SerializeField]
    private float Decel = 30f;
    [SerializeField]
    private float HolizontalSpeedThreshold = 0.05f;
    [SerializeField]
    private float FallSpeedThreshold = -0.1f;

    [Header("ジャンプ関連")]
    [SerializeField] 
    private float jumpPower = 10f;
    [SerializeField] 
    private float fallMultiplier = 3.2f;
    [SerializeField]
    private float jumpBufferTime = 0.1f;
    [SerializeField]
    private float maxFallSpeed = -18f;

    [SerializeField] 
    private Transform groundCheck;
    [SerializeField]
    private Vector2 groundCheckSize = new Vector2(0.8f, 0.1f);
    [SerializeField] 
    private float groundCheckDistance = 0.1f;
    [SerializeField] 
    private LayerMask groundLayer;

    [SerializeField]
    private float knockBackXPower = 2f;
    [SerializeField]
    private float knockBackYPower = 3f;
    [SerializeField] 
    private float knockBackControlLockTime = 0.25f;

    [ContextMenu("Die")]
    private void DebugDie()
    {
        TakeDamage(999, transform.localPosition + new Vector3(1, 0, 0));
    }

    enum State
    {
        Idle,
        Run,
        Attack,
        Hit,
        Die
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animEffect = GetComponent<AnimationEffectController>();
        bodyCollider = GetComponent<Collider2D>();
        animator = GetComponentInChildren<Animator>();
    }

    void Start()
    {

        currentHP = maxHP;
        hpBar.SetHP(currentHP, maxHP);

        transform.localScale = new Vector2(rightFacing ? 1 : -1, 1);
    }

    void Update()
    {
        if (isDead) return;
        if (isKnockBacking) return;

        isGrounded = groundSensor.IsGrounded;

        // アニメ
        animator.SetFloat("HorizontalSpeed", Mathf.Abs(rb.linearVelocity.x));
        animator.SetBool("IsGrounded", isGrounded);
        animator.SetFloat("VerticalSpeed", rb.linearVelocity.y);
    }

    private void FixedUpdate()
    {
        if (isDead) return;
        if (isKnockBacking) return;

        CulcPlayerSpeed();
        ApplyBetterJump();
    }

    private void ChangeState(State newState)
    {
        currentState = newState;

        switch (newState)
        {
            case State.Idle:
                break;

            case State.Run:
                break;

            // TODO:ここら辺のState不要かもなので要確認
            case State.Hit:
                if (animator != null) animator.SetTrigger("Hit");
                break;

            case State.Die:
                if (animator != null)
                {
                    animator.SetTrigger("Die");
                    animator.SetBool("IsDead", true);
                }
                break;
        }
    }

    private void CulcPlayerSpeed()
    {
        // 入力値
        float move = isAutoMoving ? autoMoveDirection : moveInput.x;
        //float move = moveInput.x;

        // 小さい値を0に（ブレ防止）
        if (Mathf.Abs(move) < 0.1f) move = 0f;

        // 加速・減速
        float targetSpeed = move * speed;
        float currentSpeed = rb.linearVelocityX;

        if (Mathf.Abs(move) > 0.1f)
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, Accel * Time.fixedDeltaTime);
        }
        else
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0, Decel * Time.fixedDeltaTime);
        }

        if (Mathf.Abs(currentSpeed) < HolizontalSpeedThreshold)
        {
            currentSpeed = 0f;
        }

        // 移動
        rb.linearVelocity = new Vector2(currentSpeed, rb.linearVelocity.y);
    }

    /// <summary>
    /// 落下時速度の制御
    /// </summary>
    private void ApplyBetterJump()
    {
        if (isGrounded) return;

        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
        }

        if (rb.linearVelocity.y < maxFallSpeed)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, maxFallSpeed);
        }
    }

    #region InputActions

    public void OnMove(InputAction.CallbackContext context)
    {
        if (!canInput) return;
        if (isDead) return;
        moveInput = context.ReadValue<Vector2>();

        // ノックバックの移動は操作不可時間があるため見ない
        //if (isKnockBacking) return;

        rightFacing = moveInput.x > 0;

        // 向き反転
        if (moveInput.x != 0)
        {
            transform.localScale = new Vector2(rightFacing ? 1 : -1, 1);
        }

    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (!canInput) return;
        if (isDead) return;
        if (isKnockBacking) return;
        if (!context.started) return;

        TryAttack();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        // TODO：コヨーテタイム、ジャンプバッファ入れたい
        if (!canInput) return; 
        if (!context.started) return;
        if (isDead) return;
        if (isKnockBacking) return;

        if (isGrounded)
        {
            animator.SetTrigger("Jump");
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpPower);
        }
    }
    #endregion

    #region Animation Events

    public void Anim_AttackStart()
    {
        if (isHit) return;
        if (isDead) return;

        isAttacking = true;
        animator.SetBool("IsAttacking", true);
    }

    public void OpenComboInput()
    {
        if (isHit) return;
        if (isDead) return;

        canComboInput = true;
    }

    public void CloseComboInput()
    {
        canComboInput = false;
    }

    public void Anim_AttackEnd()
    {
        EndAttack();
    }

    #endregion

    public void TakeDamage(int damage, Vector2 attackerPosition)
    {
        if (isInvincible) return;
        if (isDead) return;

        // TODO:攻撃状態の管理を整理したタイミングで場所要調整
        CancelAttack();

        currentHP -= damage;
        hpBar.SetHP(currentHP, maxHP);

        if (currentHP <= 0)
        {
            ChangeState(State.Die);
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

        //rb.linearVelocity = Vector2.zero;
        isInvincible = false;

        animEffect.StopInvincibleBlink();
    }

    private void TryAttack()
    {
        if (!isAttacking)
        {
            StartAttack(1);
            return;
        }

        if (!canComboInput) return;
        if (comboStep >= maxComboStep) return;

        StartAttack(comboStep + 1);
    }
    private void StartAttack(int comboStep)
    {
        isAttacking = true;
        this.comboStep = comboStep;

        animator.SetBool("IsAttacking", true);
        animator.SetInteger("ComboStep", comboStep);
        animator.SetTrigger("Attack");
    }

    private void EndAttack()
    {
        isAttacking = false;
        canComboInput = false;
        comboStep = 0;

        animator.SetBool("IsAttacking", false);
        animator.SetInteger("ComboStep", comboStep);
    }

    private IEnumerator DieSequence()
    {
        // 一瞬停止
        Time.timeScale = 0f;

        yield return new WaitForSecondsRealtime(0.5f);

        // スロー
        Time.timeScale = 0.2f;

        // Dieアニメ再生
        animator.SetBool("IsDead", true);

        yield return new WaitForSecondsRealtime(1f);

        // 元に戻す
        Time.timeScale = 1f;

        yield return new WaitForSeconds(1.5f);

        GameManager.Instance.GameOver();
        //Destroy(gameObject);
    }

    public void Die()
    {
        isDead = true;
        gameObject.layer = LayerMask.NameToLayer("DeadPlayer");
        rb.linearVelocity = Vector2.zero;
        //rb.simulated = false;

        StartCoroutine(DieSequence());
    }

    public void RecoverFromHit()
    {
        isKnockBacking = false;
        isHit = false;
        animator.SetBool("IsHit", isHit);
    }

    /// <summary>
    /// ムービー等の自動歩行用
    /// </summary>
    /// <param name="direction"></param>
    public void AutoMove(float direction)
    {
        isAutoMoving = true;
        autoMoveDirection = Mathf.Clamp(direction, -1f, 1f);
    }

    /// <summary>
    /// ムービー等の自動歩行停止用
    /// </summary>
    public void AutoMoveStop()
    {
        isAutoMoving = false;
        autoMoveDirection = 0f;
        rb.linearVelocity = Vector2.zero;
    }

    /// <summary>
    /// ムービーなどで入力出来ないようにする
    /// </summary>
    /// <param name="enabled">true:入力可、false:入力不可</param>
    public void SetInputEnabled(bool enabled)
    {
        canInput = enabled;

        if (!enabled)
        {
            moveInput = Vector2.zero;
        }
    }

    private void CancelAttack()
    {
        isAttacking = false;
        canComboInput = false;
        comboStep = 0;

        animator.ResetTrigger("Attack");
        animator.SetBool("IsAttacking", false);
        animator.SetInteger("ComboStep", 0);
    }
}
