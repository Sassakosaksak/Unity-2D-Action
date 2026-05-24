using DG.Tweening;
using UnityEngine;

public class SporeSEController : MonoBehaviour
{
    private AudioSource driftSource;

    [Header("SE")]
    [SerializeField]
    private SEEntry drift;

    private void Awake()
    {
        driftSource = GetComponent<AudioSource>();
    }

    public void PlayDrift()
    {
        if (drift == null) return;
        if (drift.Clips.Length == 0) return;

        driftSource.clip = drift.Clips[Random.Range(0, drift.Clips.Length)];

        driftSource.volume = drift.Volume;
        driftSource.loop = true;

        driftSource.Play();
    }

    public void StopDrift(float fadeTime = 0.2f)
    {
        if (!driftSource.isPlaying) return;

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
}