using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerAttackController))]
[RequireComponent(typeof(PlayerDamageController))]
public class PlayerController : MonoBehaviour
{
    Rigidbody2D rb;
    AnimationEffectController animEffect;
    Animator animator;
    [SerializeField]
    PlayerGroundSensor groundSensor;
    private PlayerDamageController damageController;
    private PlayerAttackController attackController;

    Vector2 moveInput;

    public bool IsDead => damageController.IsDead;

    public Collider2D BodyCollider => damageController.BodyCollider;

    private bool isGrounded;
    [SerializeField]
    private bool rightFacing = true;
    private bool isAutoMoving = false;
    private float autoMoveDirection = 1f;
    private bool canInput = true;

    private State currentState;

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
        animator = GetComponentInChildren<Animator>();
        damageController = GetComponent<PlayerDamageController>();
        attackController = GetComponent<PlayerAttackController>();
    }

    void Start()
    {
        transform.localScale = new Vector2(rightFacing ? 1 : -1, 1);
    }

    void Update()
    {
        if (damageController.IsDead) return;
        if (damageController.IsKnockBacking) return;

        isGrounded = groundSensor.IsGrounded;

        // アニメ
        animator.SetFloat("HorizontalSpeed", Mathf.Abs(rb.linearVelocity.x));
        animator.SetBool("IsGrounded", isGrounded);
        animator.SetFloat("VerticalSpeed", rb.linearVelocity.y);
    }

    private void FixedUpdate()
    {
        if (damageController.IsDead) return;
        if (damageController.IsKnockBacking) return;

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

        if (attackController.IsAttacking)
        {
            float facingDirection = rightFacing ? 1f : -1f;

            if (Mathf.Sign(move) != facingDirection)
            {
                move = 0f;
            }
        }

        // 攻撃中は移動速度下げる
        float moveSpeedMultiplier = attackController.IsAttacking ? attackController.AttackMoveMultiplier : 1f;

        // 小さい値を0に（ブレ防止）
        if (Mathf.Abs(move) < 0.1f) move = 0f;

        // 加速・減速
        float targetSpeed = move * speed * moveSpeedMultiplier;
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

        // 攻撃終了後は移動方向へ向き直す
        if (!attackController.IsAttacking && !IsInAttackAnimation())
        {
            UpdateFacing(move);
        }

        // 移動
        rb.linearVelocity = new Vector2(currentSpeed, rb.linearVelocity.y);
    }

    private bool IsInAttackAnimation()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        return stateInfo.IsTag("Attack");
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

    private void UpdateFacing(float direction)
    {
        if (Mathf.Abs(direction) < 0.1f) return;

        rightFacing = direction > 0;
        transform.localScale = new Vector2(rightFacing ? 1 : -1, 1);
    }

    #region InputActions

    public void OnMove(InputAction.CallbackContext context)
    {
        if (!canInput) return;
        if (damageController.IsDead) return;
        moveInput = context.ReadValue<Vector2>();

        // 攻撃中は向き変更禁止
        if (attackController.IsAttacking) return;

        // ノックバックの移動は操作不可時間があるため見ない
        //if (isKnockBacking) return;
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (!canInput) return;
        if (damageController.IsDead) return;
        if (damageController.IsKnockBacking) return;
        if (!context.started) return;

        attackController.HandleAttackInput();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        // TODO：コヨーテタイム、ジャンプバッファ入れたい
        if (!canInput) return; 
        if (!context.started) return;
        if (damageController.IsDead) return;
        if (damageController.IsKnockBacking) return;

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
        attackController.Anim_AttackStart();
    }

    public void OpenComboInput()
    {
        attackController.OpenComboInput();
    }

    public void CloseComboInput()
    {
        attackController.CloseComboInput();
    }

    public void Anim_AttackEnd()
    {
        attackController.Anim_AttackEnd();
    }

    #endregion

    public void TakeDamage(int damage, Vector2 attackerPosition)
    {
        damageController.TakeDamage(damage, attackerPosition);
    }

    public void RecoverFromHit()
    {
        damageController.RecoverFromHit();
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

    public void CancelAttack()
    {
        attackController.CancelAttack();
    }
}
