using UnityEngine;

public class EffectManager : MonoBehaviour
{
    public static EffectManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public void Play(GameObject effectPrefab, Vector3 position)
    {
        Instantiate(effectPrefab, position, Quaternion.identity);
    }
}