using UnityEngine;

public class SEManager : MonoBehaviour
{
    public static SEManager Instance { get; private set; }

    /// <summary>
    /// 足りなかった場合はSEManagerオブジェクトの子オブジェクトを増やす
    /// </summary>
    private AudioSource[] seSources;

    private int currentIndex = 0;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        seSources = GetComponentsInChildren<AudioSource>();
    }

    /// <summary>
    /// SEの受け口
    /// ランダム要素を決定
    /// </summary>
    /// <param name="entry"></param>
    public void Play(SEEntry entry)
    {
        if (entry == null) return;
        if (entry.Clips == null || entry.Clips.Length == 0) return;

        AudioClip clip = entry.Clips[Random.Range(0, entry.Clips.Length)];

        float pitch = 1f;

        if (entry.IsPitchRandom)
        {
            pitch = Random.Range(entry.PitchRange.x, entry.PitchRange.y);
        }

        Play(clip, entry.Volume, pitch);
    }

    /// <summary>
    /// 実際の再生処理
    /// </summary>
    /// <param name="clip"></param>
    /// <param name="volume"></param>
    /// <param name="pitch"></param>
    private void Play(AudioClip clip, float volume = 1f, float pitch = 1f)
    {
        if (clip == null) return;
        if (seSources == null || seSources.Length == 0) return;

        AudioSource source = GetAvailableSource();

        source.pitch = pitch;
        source.PlayOneShot(clip, volume);
    }

    private AudioSource GetAvailableSource()
    {
        foreach (AudioSource source in seSources)
        {
            if (!source.isPlaying)
            {
                return source;
            }
        }

        AudioSource fallbackSource = seSources[currentIndex];

        currentIndex++;

        if (currentIndex >= seSources.Length)
        {
            currentIndex = 0;
        }

        return fallbackSource;
    }
}