using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAnimationEventRelay : MonoBehaviour
{
    private PlayerController player;

    // TODO:ïsóvÇ»ÇÁè¡Ç∑
    private void Awake()
    {
        player = GetComponentInParent<PlayerController>();
    }

    //public void DieEnd()
    //{
    //    player.Anim_DieEnd();
    //}

    //public void AttackEnd()
    //{
    //    player.Anim_AttackEnd();
    //}

    //public void StunStart()
    //{
    //    player.Anim_StunStart();
    //}

    //public void StunEnd()
    //{
    //    player.Anim_StunEnd();
    //}

    public void SpawnSpore()
    {
        //mush.SpawnSpore(); 
    }
}