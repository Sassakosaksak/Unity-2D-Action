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

    [Header("Filter")]
    [SerializeField]
    private AudioLowPassFilter lowPassFilter;
    [SerializeField]
    private float defaultLowPassCutoffFrequency = 22000f;

    [Header("Volume")]
    [SerializeField]
    private float bgmVolume = 0.5f;
    [SerializeField]
    private float ambientVolume = 0.6f;
    [SerializeField]
    private float fadeDuration = 1f;

    private Tween bgmTween;
    private Tween ambientTween;
    private Tween lowPassTween;

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
        if (newBgm == null)
        {
            bgmTween = bgmSource.DOFade(0f, fadeDuration)
                                .SetUpdate(true)
                                .OnComplete(StopBGM);
            return;
        }
        if (bgmSource.clip == newBgm && bgmSource.isPlaying) return;

        bgmTween?.Kill();

        bgmTween = DOTween.Sequence()
                          .SetUpdate(true)
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
        if (newAmbient == null)
        {
            bgmTween = bgmSource.DOFade(0f, fadeDuration)
                                .SetUpdate(true)
                                .OnComplete(StopAmbient);
            return;
        }
        if (ambientSource.clip == newAmbient) return;

        ambientTween?.Kill();

        ambientTween = DOTween.Sequence()
                              .SetUpdate(true)
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

    public void StopBGM()
    {
        bgmSource.Stop();
    }

    public void StopAmbient()
    {
        ambientSource.Stop();
    }

    public void ChangeLowPassCutoffFrequency(float cutoffFrequency)
    {
        if (lowPassFilter == null) return;
        
        // ñ¢ê›íË(-1)éû
        float targetFrequency = cutoffFrequency < 0f
                                ? defaultLowPassCutoffFrequency
                                : cutoffFrequency;

        lowPassTween?.Kill();

        lowPassTween = DOTween.To(
            () => lowPassFilter.cutoffFrequency,
            value =>
            {
                if (lowPassFilter != null)
                {
                    lowPassFilter.cutoffFrequency = value;
                }
            },
            targetFrequency,
            fadeDuration
        );
    }

    public void ResetLowPassCutoffFrequency()
    {
        ChangeLowPassCutoffFrequency(defaultLowPassCutoffFrequency);
    }

}