using UnityEngine;

[CreateAssetMenu(
    fileName = "EnvironmentProfile",
    menuName = "Game/Environment Profile"
)]
public class EnvironmentProfile : ScriptableObject
{
    [Header("Audio")]
    public AudioClip bgmClip;
    public AudioClip ambientClip;
    [SerializeField]
    private float lowPassCutOffFrequency = -1f;
    public float LowPassCutOffFrequency => lowPassCutOffFrequency;

    [Header("Lighting")]
    public GameObject lightingPrefab;

    [Header("Particles")]
    public GameObject particlePrefab;
}