using UnityEngine;

/*
    セットしたColliderのTriggerに触れたタイミングで
    そのエリアの環境を変更する
    例
    ・Audio
    ・Lighting
    ・Particle
 */
public class AreaTrigger : MonoBehaviour
{
    [SerializeField]
    private EnvironmentProfile profile;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        EnvironmentManager.Instance.Apply(profile);
    }
}