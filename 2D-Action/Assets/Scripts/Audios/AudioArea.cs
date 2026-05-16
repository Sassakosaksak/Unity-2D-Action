using UnityEngine;

public class AudioArea : MonoBehaviour
{
    [SerializeField] 
    private AudioClip bgmClip;
    [SerializeField] 
    private AudioClip ambientClip;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        AudioManager.Instance.ChangeBGM(bgmClip);
        AudioManager.Instance.ChangeAmbient(ambientClip);
    }
}