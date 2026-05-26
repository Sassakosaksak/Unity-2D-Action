using DG.Tweening;
using UnityEngine;

public class ChestSEController : MonoBehaviour
{
    [Header("SE")]
    [SerializeField]
    private SEEntry Open;
    [SerializeField]
    private SEEntry Empty;

    private SEManager SE => SEManager.Instance;

    public void PlayOpen()
    {
        SE.Play(Open);
    }

    public void PlayEmpty()
    {
        SE.Play(Empty);
    }
}