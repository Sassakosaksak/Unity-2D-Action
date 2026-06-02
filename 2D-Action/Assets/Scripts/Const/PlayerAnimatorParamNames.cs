// NOTE: AnimationのParamはパフォーマンスの問題が出たタイミングで
// StringToHashにすることを検討
// 現状はTypo防止でConstに格納
public static class PlayerAnimatorParamNames
{
    public const string HorizontalSpeed = "HorizontalSpeed";
    public const string IsGrounded = "IsGrounded";
    public const string VerticalSpeed = "VerticalSpeed";
    public const string Jump = "Jump";
    public const string IsAttacking = "IsAttacking";
    public const string ComboStep = "ComboStep";
    public const string Attack = "Attack";
    public const string IsHit = "IsHit";
    public const string Die = "Die";
    public const string IsDead = "IsDead";
}
