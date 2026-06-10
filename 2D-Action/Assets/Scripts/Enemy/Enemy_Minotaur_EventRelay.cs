using UnityEngine;

public class Enemy_Minotaur_EventRelay : MonoBehaviour
{
    private Enemy_Minotaur minotaur;

    private void Awake()
    {
        minotaur = GetComponentInParent<Enemy_Minotaur>();
    }

    public void Attack1Start()
    {
        minotaur.Anim_Attack1Start();
    }

    public void Attack2Start()
    {
        minotaur.Anim_Attack2Start();
    }

    public void Attack1End()
    {
        minotaur.Anim_Attack1End();
    }

    public void Attack2End()
    {
        minotaur.Anim_Attack2End();
    }
}
