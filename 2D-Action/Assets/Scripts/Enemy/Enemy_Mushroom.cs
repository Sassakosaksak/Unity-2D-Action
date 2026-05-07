using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Enemy_Mushroom : BaseEnemyController
{
    private enum State
    {
        Idle,
        Run,
        PrepareAttack,
        Attack,
        Stun,
        Die
    }

    [Header("行動設定")]
    [SerializeField]
    private float detectRange = 4f;
    [SerializeField]
    private float walkSpeed = 0.5f;
    [SerializeField]
    private float runSpeed = 1.5f;
    [SerializeField]
    private float attackSpeed = 1f;
    [SerializeField]
    private float attackRange = 1.5f;
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
    private float attackDirection;

    private bool hasInitialized = true;

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

                //case State.PrepareAttack:
                //    UpdatePrepareAttack();
                //    break;

                //case State.Attack:
                //    UpdateAttack();
                //    break;
        }
    }

    void UpdateIdle()
    {
        float distance = GetDistanceToPlayer();

        // Idle状態でゆっくりプレイヤー側に移動
        // ToDo:回遊にしたい
        //Vector2 dir = (player.position - transform.position).normalized;
        //rb.linearVelocity = new Vector2(dir.x * walkSpeed, rb.linearVelocityY);

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

        // 発見距離の2倍離れたらRun解除
        if (distance > detectRange * 2f)
        {
            ChangeState(State.Idle);
            return;
        }

        // 攻撃距離内 かつ クールダウン経過後
        if (distance < attackRange && attackTimer > attackCooldown)
        {
            ChangeState(State.PrepareAttack);
        }
    }

    #region AnimationEvents

    public void Anim_AttackStart()
    {
        ChangeState(State.Attack);
    }

    public void Anim_AttackEnd()
    {
        // 攻撃終了時に移動停止
        rb.linearVelocity = Vector2.zero;

        attackTimer = 0f;

        if (animator.GetBool("IsStun"))
        {
            return;
        }
        CommonMoveSetting();
    }

    public void Anim_StunStart()
    {
        rb.linearVelocity = Vector2.zero;
        Debug.Log("Stunした");

    }

    public void Anim_StunEnd()
    {
        if (animator != null) animator.SetBool("IsStun", false);
        Debug.Log(animator.GetBool("IsStun"));
        CommonMoveSetting();
    }

    #endregion

    void PrepareAttack()
    {
        rb.linearVelocity = Vector2.zero;

        if (animator != null)
        {
            // 攻撃方向の保持
            attackDirection = Mathf.Sign(player.position.x - transform.position.x);
        }
    }

    void Attack()
    {
        // 保持済みの方向で移動しながら攻撃
        rb.linearVelocity = new Vector2(attackDirection * attackSpeed, rb.linearVelocityY);

        // 確率で自身がスタン
        if (true)
        {
            ChangeState(State.Stun);
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
                if (hasInitialized)
                {
                    hasInitialized = false;
                    StartCoroutine(Wait());
                }
                // 処理はUpdate内
                break;

            case State.Run:
                if (animator != null) animator.SetBool("IsDetect", true);
                // 処理はUpdate内
                break;

            case State.PrepareAttack:
                animator.SetTrigger("PrepareAttack");
                PrepareAttack();
                break;

            case State.Attack:
                if (animator != null) animator.SetTrigger("Attack");
                Attack();
                break;

            case State.Stun:
                animator.SetBool("IsStun", true);
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

    /// <summary>
    /// IdleとRunの自動判定 + State変更
    /// </summary>
    void CommonMoveSetting()
    {
        // 距離に応じてIdle or Runに戻る
        float distance = GetDistanceToPlayer();
        if (distance > detectRange)
        {
            ChangeState(State.Idle);
        }
        else
        {
            ChangeState(State.Run);
        }
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
