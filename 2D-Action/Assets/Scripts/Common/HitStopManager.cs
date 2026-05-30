using System.Collections;
using UnityEngine;

public class HitStopManager : MonoBehaviour
{
    public static HitStopManager Instance { get; private set; }

    private Coroutine hitStopCoroutine;
    private float previousTimeScale = 1f;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void Play(float duration, float timeScale)
    {
        if (hitStopCoroutine != null) return;

        hitStopCoroutine = StartCoroutine(HitStop(duration, timeScale));
    }

    private IEnumerator HitStop(float duration, float timeScale)
    {
        previousTimeScale = Time.timeScale;
        Time.timeScale = timeScale;

        yield return new WaitForSecondsRealtime(duration);

        Time.timeScale = previousTimeScale;
        hitStopCoroutine = null;
    }
}