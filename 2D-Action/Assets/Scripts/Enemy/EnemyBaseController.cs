using System.Collections;
using UnityEngine;

public class BaseEnemyController : MonoBehaviour
{
    [Header("ステータス")]
    public int maxHP = 10;
    protected int currentHP;

    [Header("参照")]
    protected Transform player;
    protected Rigidbody2D rb;
    protected Animator animator;

    protected bool isDead = false;
    protected bool isMove = false;

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
        animator = GetComponent<Animator>();
    }

    protected virtual void Start()
    {
        currentHP = maxHP;

        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }

    protected virtual void Update()
    {
        if (isDead) return;
    }

    public virtual void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHP -= damage;

        OnDamage();

        if (currentHP <= 0)
        {
            Die();
        }
    }

    protected virtual void OnDamage()
    {
        if (animator != null)
        {
            animator.SetTrigger("Hit");
        }
    }

    protected virtual void Die()
    {
        isDead = true;

        if (animator != null)
        {
            animator.SetTrigger("Die");
        }

        // TODO:Dieアニメーション後にDestroyするように修正
        Destroy(gameObject, 1.5f);
    }

    protected float GetDistanceToPlayer()
    {
        if (player == null) return Mathf.Infinity;
        return Vector2.Distance(transform.position, player.position);
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
}