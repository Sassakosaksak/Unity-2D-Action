using DG.Tweening;
using UnityEngine;

public class SporeSEController : MonoBehaviour
{
    private AudioSource driftSource;
    private Camera mainCamera;
    private Tween volumeTween;
    private bool isAudible = true;

    [Header("SE")]
    [SerializeField]
    private SEEntry drift;

    [SerializeField]
    private float audibleDistanceFromCamera = 10f;
    [SerializeField]
    private float fadeTime = 0.2f;

    private void Awake()
    {
        driftSource = GetComponent<AudioSource>();
        mainCamera = Camera.main;
    }
    private void Update()
    {
        UpdateDriftAudibleByCameraDistance();
    }

    public void PlayDrift()
    {
        if (drift == null) return;
        if (drift.Clips.Length == 0) return;

        driftSource.clip = drift.Clips[Random.Range(0, drift.Clips.Length)];

        driftSource.volume = drift.Volume;
        driftSource.loop = true;

        driftSource.Play();

        isAudible = true;
    }

    public void StopDrift(float fadeTime = 0.2f)
    {
        if (!driftSource.isPlaying) return;

        volumeTween?.Kill();

        driftSource
            .DOFade(0f, fadeTime)
            .OnComplete(() =>
            {
                driftSource.Stop();

                driftSource.volume = drift.Volume;
                driftSource.clip = null;
                driftSource.loop = false;
            });
    }

    private void UpdateDriftAudibleByCameraDistance()
    {
        if (drift == null) return;
        if (!driftSource.isPlaying) return;
        if (mainCamera == null) return;

        float distance = Vector2.Distance(transform.position, mainCamera.transform.position);

        bool shouldBeAudible = distance <= audibleDistanceFromCamera;

        if (shouldBeAudible == isAudible) return;

        isAudible = shouldBeAudible;

        float targetVolume = shouldBeAudible ? drift.Volume : 0f;

        volumeTween?.Kill();

        volumeTween = driftSource.DOFade(targetVolume, fadeTime);
    }
}