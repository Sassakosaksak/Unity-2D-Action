using UnityEngine;

public class Enemy_Mushroom_EventRelay : MonoBehaviour
{
    private Enemy_Mushroom mush;

    private void Awake()
    {
        mush = GetComponentInParent<Enemy_Mushroom>();
    }

    public void AttackStart()
    {
        mush.Anim_AttackStart();
    }

    public void AttackEnd()
    {
        mush.Anim_AttackEnd();
    }

    public void StunStart()
    {
        mush.Anim_StunStart();
    }

    public void StunEnd()
    {
        mush.Anim_StunEnd();
    }

    public void SpawnSpore()
    {
        mush.SpawnSpore();
    }
}