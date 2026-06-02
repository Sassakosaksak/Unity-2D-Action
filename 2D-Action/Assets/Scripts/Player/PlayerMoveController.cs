using UnityEngine;

public class PlayerMoveController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator animator;
    private PlayerDamageController damageController;
    private PlayerAttackController attackController;

    private Vector2 moveInput;
    private bool isGrounded;
    private bool isAutoMoving;
    private float autoMoveDirection = 1f;

    [Header("Movement")]
    [SerializeField]
    private bool rightFacing = true;
    [SerializeField]
    private float speed = 10f;
    [SerializeField]
    private float Accel = 20f;
    [SerializeField]
    private float Decel = 30f;
    [SerializeField]
    private float HolizontalSpeedThreshold = 0.05f;

    [Header("Jump")]
    [SerializeField]
    private float jumpPower = 10f;
    [SerializeField]
    private float fallMultiplier = 3.2f;
    [SerializeField]
    private float maxFallSpeed = -18f;

    [Header("Ground Check")]
    [SerializeField]
    private PlayerGroundSensor groundSensor;

    public bool IsGrounded => isGrounded;
    public bool RightFacing => rightFacing;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        damageController = GetComponent<PlayerDamageController>();
        attackController = GetComponent<PlayerAttackController>();
    }

    private void Start()
    {
        transform.localScale = new Vector2(rightFacing ? 1 : -1, 1);
    }

    private void Update()
    {
        if (damageController.IsDead) return;
        if (damageController.IsKnockBacking) return;

        isGrounded = groundSensor.IsGrounded;

        // アニメ
        animator.SetFloat(PlayerAnimatorParamNames.HorizontalSpeed, Mathf.Abs(rb.linearVelocity.x));
        animator.SetBool(PlayerAnimatorParamNames.IsGrounded, isGrounded);
        animator.SetFloat(PlayerAnimatorParamNames.VerticalSpeed, rb.linearVelocity.y);
    }

    private void FixedUpdate()
    {
        if (damageController.IsDead) return;
        if (damageController.IsKnockBacking) return;

        CulcPlayerSpeed();
        ApplyBetterJump();
    }

    public void SetMoveInput(Vector2 input)
    {
        moveInput = input;
    }

    public void Jump()
    {
        // TODO: コヨーテタイム、ジャンプバッファ入れたい
        if (!isGrounded) return;

        animator.SetTrigger(PlayerAnimatorParamNames.Jump);
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpPower);
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
        if (!enabled)
        {
            moveInput = Vector2.zero;
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
}
