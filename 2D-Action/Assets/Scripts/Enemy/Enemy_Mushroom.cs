using UnityEngine;
using UnityEngine.InputSystem;

public class Enemy_Mushroom : BaseEnemyController
{
    private enum State
    {
        Idle,
        Run,
        Attack,
        Stun,
        Die
    }

    [Header("行動設定")]
    [SerializeField]
    private float detectRange = 3f;
    [SerializeField]
    private float walkSpeed = 2f;
    [SerializeField]
    private float runSpeed = 2f;
    [SerializeField]
    private float attackRange = 2f;
    [SerializeField]
    private float attackCooldown = 2f;
    [SerializeField]
    private float stunPercentage = 0.25f;

    [SerializeField]
    private GameObject sporePrefab;
    [SerializeField]
    private Transform firePoint;

    private State currentState;
    private float attackTimer;

    private bool IsInit = true;

    protected override void Start()
    {
        base.Start();
        ChangeState(State.Idle);
    }


    protected override void Update()
    {
        base.Update();
        if (isDead) return;

        attackTimer += Time.deltaTime;

        switch (currentState)
        {
            case State.Idle:
                UpdateIdle();
                break;

            case State.Run:
                UpdateRun();
                break;

            case State.Attack:
                UpdateAttack();
                break;

        }
    }

    void UpdateIdle()
    {
        float distance = GetDistanceToPlayer();

        // Idle状態でゆっくりプレイヤー側に移動
        Vector2 dir = (player.position - transform.position).normalized;
        rb.linearVelocity = new Vector2(dir.x * walkSpeed, rb.linearVelocityY);

        // プレイヤーとの距離が一定以下になったらRun
        if (distance < detectRange)
        {
            ChangeState(State.Run);
        }
    }

    void UpdateRun()
    {
        float distance = GetDistanceToPlayer();

        // プレイヤー方向に移動
        Vector2 dir = (player.position - transform.position).normalized;
        rb.linearVelocity = new Vector2(dir.x * runSpeed, rb.linearVelocityY);

        if (distance < attackRange)
        {
            ChangeState(State.Attack);
        }
        // 発見距離の2倍離れたらRun解除
        else if (distance < detectRange * 2f)
        {
            ChangeState(State.Idle);
        }
    }

    void UpdateAttack()
    {
        // 動きを止めてその場で攻撃
        rb.linearVelocity = Vector2.zero;

        if (attackTimer >= attackCooldown)
        {
            Attack();
            attackTimer = 0f;
        }

        float distance = GetDistanceToPlayer();
        if (distance < detectRange)
        {
            ChangeState(State.Run);
        }
    }

    void Attack()
    {
        if (animator != null)
        {
            animator.SetTrigger("Attack");

            // 確率で自身がスタン
            if (Random.value < stunPercentage)
            {
                animator.SetTrigger("IsStun");
            }
        }
    }


    void ChangeState(State newState)
    {
        currentState = newState;

        switch (newState)
        {
            case State.Idle:
                if (animator != null) animator.SetBool("IsDetect", false);
                // 初回のみその場で一時待機
                if (!IsInit)
                {
                    IsInit = false;
                    StartCoroutine(Wait());
                }
                break;

            case State.Run:
                if (animator != null) animator.SetBool("IsDetect", true);
                break;

            case State.Attack:
                rb.linearVelocity = Vector2.zero;
                break;
        }
    }

    protected override void OnDamage()
    {
        base.OnDamage();

        // ノックバック+無敵時間付与
        rb.linearVelocity = Vector2.zero;
    }

    protected override void Die()
    {
        base.Die();
        ChangeState(State.Die);
    }

    public void SpawnSpore()
    {
        Debug.Log("胞子発射！");

        Vector2 dir = (player.position - transform.position).normalized;

        GameObject sporeObject = Instantiate(sporePrefab, firePoint.position, Quaternion.identity);

        Spore spore = sporeObject.GetComponent<Spore>();
        spore.Init(dir, Level);
    }
}
