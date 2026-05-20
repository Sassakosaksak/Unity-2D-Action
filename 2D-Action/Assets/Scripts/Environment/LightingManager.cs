using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightingManager : MonoBehaviour
{
    public static LightingManager Instance { get; private set; }

    [SerializeField]
    private Transform environmentRoot;

    [SerializeField]
    private float fadeDuration = 2f;

    private GameObject currentLightingObject;

    private GameObject currentLightingPrefab;

    private Tween lightingTween;

    /// <summary>
    /// LightingのIntensityのObject上の初期値を保存しておく
    /// 
    /// NOTE:
    /// 現在はPrefabを毎回Instantiateしているため未使用。
    /// LightingをキャッシュしてSetActive切り替え方式にした場合、
    /// FadeOutで0になったIntensityを復元するために再利用予定。
    /// </summary>
    private Dictionary<Light2D, float> defaultIntensityMap = new();

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void ChangeLighting(GameObject newLightingPrefab)
    {
        if (currentLightingPrefab == newLightingPrefab) return;

        lightingTween?.Kill();

        if (currentLightingObject != null)
        {
            Destroy(currentLightingObject);
            currentLightingObject = null;
        }

        currentLightingPrefab = newLightingPrefab;

        if (newLightingPrefab == null) return;

        currentLightingObject = Instantiate(
            newLightingPrefab,
            environmentRoot
        );

        FadeInLights(currentLightingObject);

        // TODO:Prefabごとに生成済みObjectをキャッシュして、SetActive切り替え方式へ変更する。
    }

    private void FadeOutLights(
        GameObject root,
        TweenCallback onComplete
    )
    {
        Light2D[] lights = root.GetComponentsInChildren<Light2D>(true);

        Sequence sequence = DOTween.Sequence();

        foreach (Light2D light in lights)
        {
            sequence.Join(
                DOTween.To(
                    () => light.intensity,
                    x => light.intensity = x,
                    0f,
                    fadeDuration
                )
            );
        }

        if (onComplete != null)
        {
            sequence.OnComplete(onComplete);
        }

        lightingTween = sequence;
    }

    private void FadeInLights(GameObject root)
    {
        // キャッシュ切り替え方式に戻した場合に使用予定
        // CacheDefaultIntensity(root);

        Light2D[] lights = root.GetComponentsInChildren<Light2D>(true);

        Sequence sequence = DOTween.Sequence();

        foreach (Light2D light in lights)
        {
            // Prefabに設定されているIntensityを目標値として使う
            float targetIntensity = light.intensity;

            // キャッシュ方式時はこちらを使用
            // float targetIntensity = defaultIntensityMap[light];

            // 0から元のIntensityまでフェードインさせる
            light.intensity = 0f;

            sequence.Join(
                DOTween.To(
                    () => light.intensity,
                    x => light.intensity = x,
                    targetIntensity,
                    fadeDuration
                )
            );
        }

        lightingTween = sequence;
    }

    /*
    /// <summary>
    /// LightingをキャッシュしてSetActive切り替えする場合に使用
    /// FadeOut後に0になったIntensityを復元するため、
    /// 初回のみ元のIntensityを保存しておく
    /// </summary>
    private void CacheDefaultIntensity(GameObject root)
    {
        Light2D[] lights = root.GetComponentsInChildren<Light2D>(true);

        // 初めて使うLightingのIntensityをDictionaryにキャッシュしておく
        foreach (Light2D light in lights)
        {
            if (!defaultIntensityMap.ContainsKey(light))
            {
                defaultIntensityMap.Add(light, light.intensity);
            }
        }
    }
    */
}