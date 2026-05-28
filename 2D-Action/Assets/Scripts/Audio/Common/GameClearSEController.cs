using DG.Tweening;
using UnityEngine;

public class GameClearSEController : MonoBehaviour
{
    [Header("SE")]
    [SerializeField]
    private SEEntry triumph;

    public void PlayTriumph()
    {
        SEManager.Instance.Play(triumph);
    }
}