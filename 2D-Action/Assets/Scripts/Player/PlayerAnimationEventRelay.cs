using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAnimationEventRelay : MonoBehaviour
{
    private PlayerController player;
    private PlayerSEController playerSE;

    private void Awake()
    {
        player = GetComponentInParent<PlayerController>();
        playerSE = GetComponentInParent<PlayerSEController>();
    }

    #region Common

    public void FootstepSE()
    {
        playerSE.PlayFootstep();
    }
    public void JumpSE()
    {
        playerSE.PlayJump();
    }

    // PlayerSEController‚ÅŽÀŽ{‚Ì‚½‚ß•s—v
    //public void LandSE()
    //{
    //    playerSE.PlayLand();
    //}

    public void DieSE()
    {
        playerSE.PlayDeath();
    }

    #endregion

    #region Attack
    public void AttackStart()
    {
        player.Anim_AttackStart();
    }

    public void AttackComboInputOpen()
    {
        player.OpenComboInput();
    }

    public void AttackComboInputClose()
    {
        player.CloseComboInput();
    }

    public void AttackSE()
    {
        playerSE.PlaySwordSwing();
    }

    public void AttackEnd()
    {
        player.Anim_AttackEnd();
    }

    #endregion

    #region Hit

    public void HitSE()
    {
        playerSE.PlayDamage();
    }

    public void HitEnd()
    {
        player.RecoverFromHit();
    }

    #endregion
}