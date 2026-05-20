using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    public static ParticleManager Instance { get; private set; }

    [SerializeField]
    private Transform environmentRoot;

    private GameObject currentParticleObject;
    private GameObject currentParticlePrefab;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void ChangeParticle(GameObject particlePrefab)
    {
        if (currentParticlePrefab == particlePrefab) return;

        if (currentParticleObject != null)
        {
            Destroy(currentParticleObject);
        }

        currentParticlePrefab = particlePrefab;

        if (particlePrefab == null) return;

        currentParticleObject = Instantiate(
            particlePrefab,
            environmentRoot
        );
    }
}