using UnityEngine;

public class PlayerSEController : MonoBehaviour
{
    private PlayerGroundSensor groundSensor;

    [Header("PlayerSE")]
    [SerializeField]
    private SEEntry swordSwing;

    [SerializeField]
    private SurfaceSESet[] footsteps;

    [SerializeField]
    private SEEntry jump;

    [SerializeField]
    private SEEntry land;

    [SerializeField]
    private SEEntry damage;

    [SerializeField]
    private SEEntry damageVoice;

    [SerializeField]
    private SEEntry deathVoice;

    private SEManager SE => SEManager.Instance;

    /// <summary>
    /// 1F前接地していたかのフラグ
    /// </summary>
    private bool wasGrounded = false;
    
    /// <summary>
    /// 初期化時landSEがならないようにするフラグ
    /// </summary>
    private bool hasGroundCheckInitialized = false;

    private void Awake()
    {
        groundSensor = GetComponentInChildren<PlayerGroundSensor>();
    }

    public void PlaySwordSwing()
    {
        SE.Play(swordSwing);
    }
    private void Update()
    {
        CheckLandSE();
    }

    public void PlayFootstep()
    {
        SurfaceType currentSurface = groundSensor.CurrentSurface;

        foreach(SurfaceSESet footstep in footsteps)
        {
            if(footstep.SurfaceType != currentSurface)
            {
                continue;
            }
            SE.Play(footstep.Se);
            return;
        }
    }

    public void PlayJump()
    {
        SE.Play(jump);
    }

    /// <summary>
    /// 1F前が空中の場合着地音を鳴らす処理
    /// </summary>
    private void CheckLandSE()
    {
        bool isGrounded = groundSensor.IsGrounded;

        if (hasGroundCheckInitialized && !wasGrounded && isGrounded)
        {
            PlayLand();
        }

        wasGrounded = isGrounded;
        hasGroundCheckInitialized = true;
    }

    public void PlayLand()
    {
        SE.Play(land);
    }

    public void PlayDamage()
    {
        //SE.Play(damage);
        //SE.Play(damageVoice);
    }

    public void PlayDeath()
    {
        SE.Play(damage);
        SE.Play(deathVoice);
    }
}