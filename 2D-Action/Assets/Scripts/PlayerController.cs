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
    private float invincibleTime = 1f;


    public float speed = 10f;
    [SerializeField]
    private float Accel = 20f;
    [SerializeField]
    private float Decel = 30f;

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

        culcPlayerSpeed();

        // アニメ
        animator.SetFloat("Speed", Mathf.Abs(moveInput.x));
    }

    private void culcPlayerSpeed()
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
            currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, Accel * Time.deltaTime);
        }
        else
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0, Decel * Time.deltaTime);
        }

        // 移動
        rb.linearVelocity = new Vector2(currentSpeed, rb.linearVelocity.y);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();

        // 向き反転
        if (moveInput.x != 0)
        {
            transform.localScale = new Vector2(moveInput.x > 0 ? 1 : -1, 1);
        }

    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            animator.SetTrigger("Attack");
        }
    }

    public void TakeDamage(int damage)
    {
        if (isInvincible) return;

        currentHP -= damage;

        if (currentHP <= 0)
        {
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

        yield return new WaitForSecondsRealtime(0.05f);

        // スロー
        Time.timeScale = 0.2f;

        // Dieアニメ再生
        animator.SetTrigger("Die");

        yield return new WaitForSecondsRealtime(1f);

        // 元に戻す
        Time.timeScale = 1f;

        Destroy(gameObject);
    }

    public void Die()
    {
        isDead = true;
        DieSequence();
    }
}
