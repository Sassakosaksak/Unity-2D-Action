using UnityEngine;

public class Enemy_Minotaur : EnemyControllerBase
{
    private enum State
    {
        PatrolIdle,
        PatrolWalk,
        Chase,
        AttackWarning,
        Attack1,
        Attack2
    }

    [Header("Detection")]
    [SerializeField]
    private float detectRange = 4f;
    [SerializeField]
    private float loseRange = 5f;

    [Header("Patrol")]
    [SerializeField]
    private float patrolRange = 3f;
    [SerializeField]
    private float walkSpeed = 0.5f;
    [SerializeField]
    private Vector2 idleDurationRange = new(1f, 2.5f);
    [SerializeField]
    private Vector2 walkDurationRange = new(1f, 2f);

    [Header("Chase")]
    [SerializeField]
    private float runSpeed = 1.5f;
    [SerializeField]
    private float attackRange = 1.5f;

    [Header("Attack")]
    [SerializeField]
    private float attackCooldown = 2f;
    [SerializeField]
    [Tooltip("1段目の突進距離")]
    private float firstAttackDashDistance = 0.45f;
    [SerializeField]
    [Tooltip("1段目の突進時間")]
    private float firstAttackDashDuration = 0.12f;
    [SerializeField]
    [Tooltip("2段目の突進距離")]
    private float secondAttackDashDistance = 0.65f;
    [SerializeField]
    [Tooltip("2段目の突進時間")]
    private float secondAttackDashDuration = 0.14f;
    [SerializeField, Range(0f, 1f)]
    private float secondAttackChance = 0.5f;

    private State currentState;
    private Vector2 patrolOrigin;
    private float patrolTimer;
    private float attackCooldownTimer;
    private float attackDirection;
    private float attackDashTimer;
    private float attackDashSpeed;
    private bool useSecondAttack;

    public bool IsSecondAttack => currentState == State.Attack2;

    protected override void Start()
    {
        base.Start();

        patrolOrigin = transform.position;
        attackCooldownTimer = attackCooldown;
        EnterPatrolIdle();
    }

    protected override void Update()
    {
        if (!CanAct()) return;

        attackCooldownTimer += Time.deltaTime;

        if (IsPlayerDead())
        {
            EnterPatrolIdle();
            return;
        }

        switch (currentState)
        {
            case State.PatrolIdle:
                UpdatePatrolIdle();
                break;
            case State.PatrolWalk:
                UpdatePatrolWalk();
                break;
            case State.Chase:
                UpdateChase();
                break;
            case State.Attack1:
                UpdateAttack();
                break;
            case State.Attack2:
                UpdateAttack();
                break;
        }
    }

    private void UpdatePatrolIdle()
    {
        rb.linearVelocity = Vector2.zero;

        if (TryStartChase()) return;

        patrolTimer -= Time.deltaTime;
        if (patrolTimer <= 0f)
        {
            EnterPatrolWalk();
        }
    }

    private void UpdatePatrolWalk()
    {
        if (TryStartChase()) return;

        patrolTimer -= Time.deltaTime;
        float distanceFromOrigin = transform.position.x - patrolOrigin.x;

        bool reachedPatrolEdge =
             rightFacing && distanceFromOrigin >= patrolRange ||
            !rightFacing && distanceFromOrigin <= -patrolRange;

        if (reachedPatrolEdge || patrolTimer <= 0f)
        {
            EnterPatrolIdle();
            return;
        }

        float direction = rightFacing ? 1f : -1f;
        rb.linearVelocity = new Vector2(direction * walkSpeed, rb.linearVelocity.y);
    }

    private void UpdateChase()
    {
        float distance = GetDistanceToPlayer();
        float horizontalDistance = Mathf.Abs(player.position.x - transform.position.x);

        if (distance > loseRange)
        {
            EnterPatrolIdle();
            return;
        }

        FlipToPlayer();

        if (horizontalDistance <= attackRange &&
            attackCooldownTimer >= attackCooldown)
        {
            EnterAttackWarning();
            return;
        }

        float direction = Mathf.Sign(player.position.x - transform.position.x);
        Vector2 separation = GetSeparationVelocity();
        rb.linearVelocity = new Vector2(direction * runSpeed + separation.x, rb.linearVelocity.y);
    }

    private void UpdateAttack()
    {
        if (attackDashTimer <= 0f)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            return;
        }

        attackDashTimer -= Time.deltaTime;
        rb.linearVelocity = new Vector2(attackDirection * attackDashSpeed, rb.linearVelocity.y);

        if (attackDashTimer <= 0f)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        }
    }

    private bool TryStartChase()
    {
        if (GetDistanceToPlayer() > detectRange)
        {
            return false;
        }

        EnterChase();
        return true;
    }

    private void EnterPatrolIdle()
    {
        currentState = State.PatrolIdle;
        patrolTimer = Random.Range(idleDurationRange.x, idleDurationRange.y);
        rb.linearVelocity = Vector2.zero;
        animEffect.KillAllEffects();

        if (animator != null)
        {
            animator.SetBool(EnemyAnimatorParamNames.IsDetect, false);
        }
    }

    private void EnterPatrolWalk()
    {
        currentState = State.PatrolWalk;
        patrolTimer = Random.Range(walkDurationRange.x, walkDurationRange.y);

        float distanceFromOrigin = transform.position.x - patrolOrigin.x;
        if (distanceFromOrigin >= patrolRange)
        {
            FaceToRight(false);
        }
        else if (distanceFromOrigin <= -patrolRange)
        {
            FaceToRight(true);
        }
        else
        {
            FaceToRight(Random.value >= 0.5f);
        }

        if (animator != null)
        {
            animator.SetBool(EnemyAnimatorParamNames.IsDetect, true);
        }
    }

    private void EnterChase()
    {
        currentState = State.Chase;
        animEffect.KillAllEffects();

        if (animator != null)
        {
            animator.SetBool(EnemyAnimatorParamNames.IsDetect, true);
        }
    }

    private void EnterAttackWarning()
    {
        currentState = State.AttackWarning;
        rb.linearVelocity = Vector2.zero;

        FlipToPlayer();
        attackDirection = rightFacing ? 1f : -1f;
        useSecondAttack = Random.value < secondAttackChance;
        animator.SetBool(EnemyAnimatorParamNames.IsSecondAttack, useSecondAttack);
        animator.SetTrigger(EnemyAnimatorParamNames.PrepareAttack);
        animEffect.PlayAttackWarning();
    }

    public void Anim_Attack1Start()
    {
        currentState = State.Attack1;
        StopAttackDash();
    }

    public void Anim_Attack1DashStart()
    {
        StartAttackDash(firstAttackDashDistance, firstAttackDashDuration);
    }

    public void Anim_Attack2Start()
    {
        currentState = State.Attack2;
        StopAttackDash();
    }

    public void Anim_Attack2DashStart()
    {
        StartAttackDash(secondAttackDashDistance, secondAttackDashDuration);
    }

    public void Anim_Attack1End()
    {
        if (useSecondAttack) return;

        FinishAttack();
    }

    public void Anim_Attack2End()
    {
        FinishAttack();
    }

    private void FinishAttack()
    {
        StopAttackDash();
        attackCooldownTimer = 0f;
        animator.SetBool(EnemyAnimatorParamNames.IsSecondAttack, false);
        EnterChase();
    }

    private void StartAttackDash(float distance, float duration)
    {
        attackDashTimer = Mathf.Max(0f, duration);
        attackDashSpeed = distance / Mathf.Max(0.01f, duration);
    }

    private void StopAttackDash()
    {
        attackDashTimer = 0f;
        attackDashSpeed = 0f;
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
    }

    protected override void Hit()
    {
        if (animator == null) return;

        animator.SetTrigger(EnemyAnimatorParamNames.Hurt);
        animEffect.PlayHitFlash();
        animEffect.PlayHitPunch();
        StartCoroutine(InvincibleCoroutine());
    }

    protected override void RecoverFromHit()
    {
        base.RecoverFromHit();

        if (!isDead)
        {
            EnterChase();
        }
    }
}
