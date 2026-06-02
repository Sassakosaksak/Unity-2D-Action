using UnityEngine;

public class PlayerAttackController : MonoBehaviour
{
    private Animator animator;
    private PlayerDamageController damageController;

    private bool isAttacking;
    private int comboStep = 0;
    private bool canComboInput = false;

    [SerializeField]
    private int maxComboStep = 2;

    [SerializeField]
    private float attackMoveMultiplier = 0.3f;

    public bool IsAttacking => isAttacking;
    public float AttackMoveMultiplier => attackMoveMultiplier;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        damageController = GetComponent<PlayerDamageController>();
    }

    /// <summary>
    /// 攻撃入力を処理し、初段または次のコンボ攻撃を実行
    /// </summary>
    public void HandleAttackInput()
    {
        // 攻撃中でなければ初段を開始
        if (!isAttacking)
        {
            StartAttack(1);
            return;
        }

        // コンボ受付中かつ最大段数未満なら次段へ派生
        if (!canComboInput) return;
        if (comboStep >= maxComboStep) return;

        StartAttack(comboStep + 1);
    }

    public void StartAttack(int comboStep)
    {
        isAttacking = true;
        this.comboStep = comboStep;

        animator.SetBool(PlayerAnimatorParamNames.IsAttacking, true);
        animator.SetInteger(PlayerAnimatorParamNames.ComboStep, comboStep);
        animator.SetTrigger(PlayerAnimatorParamNames.Attack);
    }

    public void EndAttack()
    {
        isAttacking = false;
        canComboInput = false;
        comboStep = 0;

        animator.SetBool(PlayerAnimatorParamNames.IsAttacking, false);
        animator.SetInteger(PlayerAnimatorParamNames.ComboStep, comboStep);
    }

    public void CancelAttack()
    {
        isAttacking = false;
        canComboInput = false;
        comboStep = 0;

        animator.ResetTrigger(PlayerAnimatorParamNames.Attack);
        animator.SetBool(PlayerAnimatorParamNames.IsAttacking, false);
        animator.SetInteger(PlayerAnimatorParamNames.ComboStep, 0);
    }

    public void Anim_AttackStart()
    {
        if (damageController.IsHit) return;
        if (damageController.IsDead) return;

        isAttacking = true;
        animator.SetBool(PlayerAnimatorParamNames.IsAttacking, true);
    }

    public void Anim_AttackEnd()
    {
        EndAttack();
    }

    public void OpenComboInput()
    {
        if (damageController.IsHit) return;
        if (damageController.IsDead) return;

        canComboInput = true;
    }

    public void CloseComboInput()
    {
        canComboInput = false;
    }
}
