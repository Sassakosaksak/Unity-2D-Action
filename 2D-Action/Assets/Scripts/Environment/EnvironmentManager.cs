using UnityEngine;

public class EnvironmentManager : MonoBehaviour
{
    public static EnvironmentManager Instance { get; private set; }

    [SerializeField]
    private EnvironmentProfile initialProfile;

    private EnvironmentProfile currentProfile;
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        Apply(initialProfile);
    }

    public void Apply(EnvironmentProfile profile)
    {
        if (profile == null) return;

        if (currentProfile == profile) return;
        currentProfile = profile;

        AudioManager.Instance.ChangeBGM(profile.bgmClip);
        AudioManager.Instance.ChangeAmbient(profile.ambientClip);
        AudioManager.Instance.ChangeLowPassCutoffFrequency(profile.LowPassCutOffFrequency);

        LightingManager.Instance.ChangeLighting(profile.lightingPrefab);
        ParticleManager.Instance.ChangeParticle(profile.particlePrefab);
    }
}