using UnityEngine;

/*
    セットしたColliderのTriggerに触れたタイミングで
    そのエリアの環境を変更する
    例
    ・Audio
    ・Lighting
    ・Particle
 */
public class AreaTrigger : MonoBehaviour
{
    [SerializeField]
    private EnvironmentProfile profile;

    private void Awake()
    {
        PreloadProfile();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        EnvironmentManager.Instance.Apply(profile);
    }

    private void PreloadProfile()
    {
        if (profile == null) return;

        PreloadAudioClip(profile.bgmClip);
        PreloadAudioClip(profile.ambientClip);
    }

    private void PreloadAudioClip(AudioClip clip)
    {
        if (clip == null) return;

        clip.LoadAudioData();
    }
}