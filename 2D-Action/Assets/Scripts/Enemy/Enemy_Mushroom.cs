using UnityEngine;

public class Enemy_Mushroom : EnemyControllerBase
{
    private enum State
    {
        Idle,
        Run,
        PrepareAttack,
        Attack,
        Stun
    }

    [Header("Move")]
    [SerializeField]
    private float detectRange = 4f;
    [SerializeField]
    private float walkSpeed = 0.5f;
    [SerializeField]
    private float runSpeed = 1.5f;
    [SerializeField]
    private float attackSpeed = 1f;

    [Header("Combat")]
    [SerializeField]
    private float attackRange = 1.5f;
    [SerializeField]
    private float attackCooldown = 2f;
    [SerializeField]
    private float stunPercentage = 0.25f;
    [SerializeField]
    private float attackRangeIncreaseSpeed = 0.5f;

    private float currentAttackRangeBonus;
    [SerializeField]
    private GameObject sporePrefab;
    [SerializeField]
    private Transform firePoint;

    [Header("SE")]
    [SerializeField]
    private float detectSECooldown = 3f;

    private float lastDetectSETime = -999f;
    private MushroomSEController mushroomSE;

    private State currentState;
    private float attackTimer;
    private float attackDirection;

    private bool hasInitialized = true;

    protected override void Awake()
    {
        base.Awake();
        mushroomSE = GetComponent<MushroomSEController>();
    }

    protected override void Start()
    {
        base.Start();
        ChangeState(State.Idle);
    }


    protected override void Update()
    {
        if (!CanAct()) return;

        attackTimer += Time.deltaTime;

        if (IsPlayerDead())
        {
            ChangeState(State.Idle);
            rb.linearVelocity = Vector2.zero;
            return;
        }

        switch (currentState)
        {
            case State.Idle:
                UpdateIdle();
                break;

            case State.Run:
                UpdateRun();
                break;

        }
    }

    private void UpdateIdle()
    {
        float distance = GetDistanceToPlayer();

        // ToDo:回遊にしたい
        rb.linearVelocity = Vector2.zero;

        // プレイヤーとの距離が一定以下になったらRun
        if (distance < detectRange && attackTimer > attackCooldown)
        {
            ChangeState(State.Run);
        }
    }

    private void UpdateRun()
    {
        float distance = GetDistanceToPlayer();

        // プレイヤー方向に移動
        FlipToPlayer();

        currentAttackRangeBonus += attackRangeIncreaseSpeed * Time.deltaTime;
        currentAttackRangeBonus = Mathf.Min(currentAttackRangeBonus, detectRange);
        float currentAttackRange = attackRange + currentAttackRangeBonus;

        Vector2 dir = (player.position - transform.position).normalized;
        Vector2 separation = GetSeparationVelocity();
        rb.linearVelocity = new Vector2(dir.x * runSpeed + separation.x, 0f);

        // 発見距離の1.2倍離れたらRun解除
        if (distance > detectRange * 1.2f)
        {
            ChangeState(State.Idle);
            return;
        }

        // 攻撃距離内 かつ クールダウン経過後
        if (distance < currentAttackRange && attackTimer > attackCooldown)
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

        if (animator != null && animator.GetBool("IsStun"))
        {
            return;
        }
        ReturnToIdle();
    }

    public void Anim_StunStart()
    {
        rb.linearVelocity = Vector2.zero;
    }

    public void Anim_StunEnd()
    {
        if (animator != null) animator.SetBool("IsStun", false);
        ReturnToIdle();
    }

    #endregion

    private void PrepareAttack()
    {
        rb.linearVelocity = Vector2.zero;

        animEffect.PlayAttackWarning();

        if (player == null) return;

        // 攻撃方向の保持
        attackDirection = Mathf.Sign(player.position.x - transform.position.x);
    }

    private void Attack()
    {
        // 保持済みの方向で移動しながら攻撃
        rb.linearVelocity = new Vector2(attackDirection * attackSpeed, 0);
        // 確率で自身がスタン
        if (Random.value < stunPercentage)
        {
            ChangeState(State.Stun);
            return;
        }
    }

    private void ChangeState(State newState)
    {
        animEffect.KillAllEffects();

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
                ResetAttackRangeBonus(); 
                if (animator != null) animator.SetBool("IsDetect", true);

                float currentTime = Time.time;
                if(currentTime > lastDetectSETime + detectSECooldown)
                {
                    mushroomSE.PlayDetect();
                    lastDetectSETime = currentTime;
                }
                // 処理はUpdate内
                break;

            case State.PrepareAttack:
                if (animator != null) animator.SetTrigger("PrepareAttack");
                PrepareAttack();
                break;

            case State.Attack:
                if (animator != null) animator.SetTrigger("Attack");
                Attack();
                break;

            case State.Stun:
                if (animator != null) animator.SetBool("IsStun", true);
                break;
        }
    }

    private void ReturnToIdle()
    {
        ChangeState(State.Idle);
    }

    protected override void RecoverFromHit()
    {
        base.RecoverFromHit();

        ChangeState(State.Idle);
    }

    public void SpawnSpore()
    {
        Vector2 dir = rightFacing ? Vector2.right : Vector2.left;

        GameObject sporeObject = Instantiate(sporePrefab, firePoint.position, Quaternion.identity);

        Spore spore = sporeObject.GetComponent<Spore>();
        spore.Init(dir, Level);
    }
    private void ResetAttackRangeBonus()
    {
        currentAttackRangeBonus = 0f;
    }
}
