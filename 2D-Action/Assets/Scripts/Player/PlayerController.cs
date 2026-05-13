using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D rb;
    Animator animator;
    Vector2 moveInput;

    [Header("ステータス関連")]
    //[SerializeField]
    //private int playerHP = 10;
    [SerializeField]
    private int currentHP;
    private bool isInvincible;
    private bool isDead;
    public bool IsDead => isDead;
    private float invincibleTime = 1f;
    private bool isGrounded;
    private bool isAttacking;
    private bool rightFacing = true;

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

    [ContextMenu("Die")]
    private void DebugDie()
    {
        TakeDamage(999);
    }

    enum State
    {
        Idle,
        Run,
        Attack,
        Hit,
        Die
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        if (isDead) return;

        CheckGround();

        // アニメ
        animator.SetFloat("HorizontalSpeed", Mathf.Abs(rb.linearVelocity.x));
        animator.SetBool("IsGrounded", isGrounded);
        animator.SetFloat("VerticalSpeed", rb.linearVelocity.y);
    }

    private void FixedUpdate()
    {
        if (isDead) return;
        
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
        float move = moveInput.x;

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
        if (isDead) return;

        moveInput = context.ReadValue<Vector2>();

        rightFacing = moveInput.x > 0;
        // 向き反転
        if (moveInput.x != 0)
        {
            transform.localScale = new Vector2(rightFacing ? 1 : -1, 1);
        }

    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (isDead) return;

        if (context.started)
        {
            SetIsAttacking(true);
            animator.SetTrigger("Attack");
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        // TODO：コヨーテタイム、ジャンプバッファ入れたい
        if (!context.started) return;
        if (isDead) return;

        if (isGrounded)
        {
            animator.SetTrigger("Jump");
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpPower);
        }
    }
    #endregion

    #region Animation Events

    public void Anim_AttackEnd()
    {
        SetIsAttacking();
    }

    #endregion
    
    public void TakeDamage(int damage)
    {
        if (isInvincible) return;
        if (isDead) return;

        currentHP -= damage;

        if (currentHP <= 0)
        {
            ChangeState(State.Die);
            Die();
            return;
        }

        animator.SetTrigger("Hit");

        StartCoroutine(InvincibleCoroutine());
    }

    IEnumerator InvincibleCoroutine()
    {
        isInvincible = true;

        yield return new WaitForSeconds(invincibleTime);

        isInvincible = false;
    }

    private IEnumerator DieSequence()
    {
        // 一瞬停止
        Time.timeScale = 0f;

        yield return new WaitForSecondsRealtime(0.5f);

        // スロー
        Time.timeScale = 0.2f;

        // Dieアニメ再生
        animator.SetTrigger("Die");

        yield return new WaitForSecondsRealtime(1f);

        // 元に戻す
        Time.timeScale = 1f;

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

    private void CheckGround()
    {
        isGrounded = Physics2D.BoxCast(
            groundCheck.position,
            groundCheckSize,
            0f,
            Vector2.down,
            groundCheckDistance,
            groundLayer
        );
    }

    private void SetIsAttacking(bool isAttacking = false)
    {
        this.isAttacking = isAttacking;
        animator.SetBool("IsAttacking", isAttacking);
    }

    private void OnDrawGizmos()
    {
        if (groundCheck == null) return;

        Gizmos.color = Color.red;

        Vector2 boxCenter = (Vector2)groundCheck.position + Vector2.down * groundCheckDistance;

        Gizmos.DrawWireCube(boxCenter, groundCheckSize);
    }
}
