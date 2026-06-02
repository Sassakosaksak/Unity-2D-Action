using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerMoveController))]
[RequireComponent(typeof(PlayerAttackController))]
[RequireComponent(typeof(PlayerDamageController))]
public class PlayerController : MonoBehaviour
{
    private PlayerDamageController damageController;
    private PlayerAttackController attackController;
    private PlayerMoveController moveController;

    public bool IsDead => damageController.IsDead;

    public Collider2D BodyCollider => damageController.BodyCollider;

    private bool canInput = true;

    private void Awake()
    {
        damageController = GetComponent<PlayerDamageController>();
        attackController = GetComponent<PlayerAttackController>();
        moveController = GetComponent<PlayerMoveController>();
    }

    #region InputActions

    public void OnMove(InputAction.CallbackContext context)
    {
        if (!CanReceiveInput()) return;

        moveController.SetMoveInput(context.ReadValue<Vector2>());
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (!CanAct()) return;
        if (!context.started) return;

        attackController.HandleAttackInput();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (!context.started) return;
        if (!CanAct()) return;

        moveController.Jump();
    }

    #endregion

    private bool CanReceiveInput()
    {
        return canInput && !damageController.IsDead;
    }

    private bool CanAct()
    {
        return CanReceiveInput() && !damageController.IsKnockBacking;
    }

    #region Animation Events

    public void Anim_AttackStart()
    {
        attackController.Anim_AttackStart();
    }

    public void OpenComboInput()
    {
        attackController.OpenComboInput();
    }

    public void CloseComboInput()
    {
        attackController.CloseComboInput();
    }

    public void Anim_AttackEnd()
    {
        attackController.Anim_AttackEnd();
    }

    #endregion

    public void TakeDamage(int damage, Vector2 attackerPosition)
    {
        damageController.TakeDamage(damage, attackerPosition);
    }

    public void RecoverFromHit()
    {
        damageController.RecoverFromHit();
    }

    /// <summary>
    /// ムービー等の自動歩行用
    /// </summary>
    /// <param name="direction"></param>
    public void AutoMove(float direction)
    {
        moveController.AutoMove(direction);
    }

    /// <summary>
    /// ムービー等の自動歩行停止用
    /// </summary>
    public void AutoMoveStop()
    {
        moveController.AutoMoveStop();
    }

    /// <summary>
    /// ムービーなどで入力出来ないようにする
    /// </summary>
    /// <param name="enabled">true:入力可、false:入力不可</param>
    public void SetInputEnabled(bool enabled)
    {
        canInput = enabled;
        moveController.SetInputEnabled(enabled);
    }

    public void CancelAttack()
    {
        attackController.CancelAttack();
    }
}
