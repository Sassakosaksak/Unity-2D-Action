using UnityEngine;

public class SEManager : MonoBehaviour
{
    public static SEManager Instance { get; private set; }

    [SerializeField]
    private AudioSource seSource;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void Play(
        AudioClip clip,
        float volume = 0.7f/*,
        float pitch = 1f,
        bool ramdamPitch = false*/)
    {
        if (clip == null) return;

        seSource.PlayOneShot(clip, volume);
    }
}