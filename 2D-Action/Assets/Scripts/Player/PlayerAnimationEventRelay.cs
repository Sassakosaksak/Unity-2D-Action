using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAnimationEventRelay : MonoBehaviour
{
    private PlayerController player;
    private PlayerAttack playerAttack;

    private void Awake()
    {
        player = GetComponentInParent<PlayerController>();
        playerAttack = GetComponentInChildren<PlayerAttack>();
    }

    //public void DieEnd()
    //{
    //    player.Anim_DieEnd();
    //}

    public void AttackStart()
    {
        player.Anim_AttackStart();
    }

    public void AttackEnd()
    {
        player.Anim_AttackEnd();
    }

    public void HitEnd()
    {
        player.RecoverFromHit();
    }

    //public void StunEnd()
    //{
    //    player.Anim_StunEnd();
    //}
}