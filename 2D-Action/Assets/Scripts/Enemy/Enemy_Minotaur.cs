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
    private float firstAttackSpeed = 1f;
    [SerializeField]
    private float secondAttackSpeed = 1.3f;
    [SerializeField, Range(0f, 1f)]
    private float secondAttackChance = 0.5f;
    [SerializeField]
    private int secondAttackDamage = 2;

    private State currentState;
    private Vector2 patrolOrigin;
    private float patrolTimer;
    private float attackCooldownTimer;
    private float attackDirection;
    private bool useSecondAttack;

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
                UpdateAttack(firstAttackSpeed);
                break;
            case State.Attack2:
                UpdateAttack(secondAttackSpeed);
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

        if (distance > loseRange)
        {
            EnterPatrolIdle();
            return;
        }

        FlipToPlayer();

        if (distance <= attackRange && attackCooldownTimer >= attackCooldown)
        {
            EnterAttackWarning();
            return;
        }

        float direction = Mathf.Sign(player.position.x - transform.position.x);
        Vector2 separation = GetSeparationVelocity();
        rb.linearVelocity = new Vector2(direction * runSpeed + separation.x, rb.linearVelocity.y);
    }

    private void UpdateAttack(float speed)
    {
        rb.linearVelocity = new Vector2(attackDirection * speed, rb.linearVelocity.y);
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
    }

    public void Anim_Attack2Start()
    {
        currentState = State.Attack2;
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
        rb.linearVelocity = Vector2.zero;
        attackCooldownTimer = 0f;
        animator.SetBool(EnemyAnimatorParamNames.IsSecondAttack, false);
        EnterChase();
    }

    public override void BodyAttack(PlayerController targetPlayer)
    {
        if (targetPlayer == null) return;
        if (currentState != State.Attack1 && currentState != State.Attack2) return;

        int damage = currentState == State.Attack2 ? secondAttackDamage : bodyAttackDamage;
        targetPlayer.TakeDamage(damage, transform.position);
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
