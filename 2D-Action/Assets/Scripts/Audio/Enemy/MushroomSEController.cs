using UnityEngine;

public class MushroomSEController : EnemyBaseSEController
{
    [Header("SE")]
    [SerializeField]
    private SEEntry hit;

    [SerializeField]
    private SEEntry detectVoice;

    [SerializeField]
    private SEEntry damageVoice;

    [SerializeField]
    private SEEntry deathVoice;

    private SEManager SE => SEManager.Instance;

    public void PlayDetect()
    {
        SE.Play(detectVoice);
    }

    /// <summary>
    ///  ヒット音+ダメージ音声
    /// </summary>
    public override void PlayHit()
    {
        SE.Play(hit);
        SE.Play(damageVoice);
    }

    /// <summary>
    ///  ヒット音+死亡時音声
    /// </summary>
    public override void PlayDie()
    {
        SE.Play(hit);
        SE.Play(deathVoice);
    }
}