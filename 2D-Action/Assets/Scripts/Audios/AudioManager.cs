using DG.Tweening;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Source")]
    [SerializeField]
    private AudioSource bgmSource;
    [SerializeField]
    private AudioSource ambientSource;
    [SerializeField]
    private AudioSource seSource;

    [Header("Volume")]
    [SerializeField]
    private float bgmVolume = 0.5f;
    [SerializeField]
    private float ambientVolume = 0.6f;
    [SerializeField]
    private float fadeDuration = 1f;

    private Tween bgmTween;
    private Tween ambientTween;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void ChangeBGM(AudioClip newBgm)
    {
        // 新しいBGMが未指定 or 現在のBGMと同じならreturn
        if (newBgm == null) return;
        if (bgmSource.clip == newBgm && bgmSource.isPlaying) return;

        bgmTween?.Kill();

        bgmTween = DOTween.Sequence()
                          .Append(bgmSource.DOFade(0f, fadeDuration))
                          .AppendCallback(() =>
                          {
                              bgmSource.clip = newBgm;
                              bgmSource.volume = 0f;
                              bgmSource.loop = true;
                              bgmSource.Play();
                          })
                          .Append(bgmSource.DOFade(bgmVolume, fadeDuration));
    }

    public void ChangeAmbient(AudioClip newAmbient)
    {
        // 新しいBGMが未指定 or 現在のBGMと同じならreturn
        if (newAmbient == null) return;
        if (ambientSource.clip == newAmbient) return;

        ambientTween?.Kill();

        ambientTween = DOTween.Sequence()
                              .Append(ambientSource.DOFade(0f, fadeDuration))
                              .AppendCallback(() =>
                              {
                                  ambientSource.clip = newAmbient;
                                  ambientSource.volume = 0f;
                                  ambientSource.loop = true;
                                  ambientSource.Play();
                              })
                              .Append(ambientSource.DOFade(ambientVolume, fadeDuration));
    }

    public void PlaySE(AudioClip clip)
    {
        if (clip == null) return;
        seSource.PlayOneShot(clip);
    }

    public void StopBGM()
    {
        bgmSource.Stop();
    }

    public void StopAmbient()
    {
        ambientSource.Stop();
    }
}