using System.Collections;
using UnityEngine;

/*
    Animatorで指定するTransitionが長いと
    アニメーション途中にDestroyする可能性あり
    基本的にTransitionは0に近い値にすること
 */

public class EffectController : MonoBehaviour
{
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private float DestroyMargin = 0.2f;

    private void Start()
    {
        Play();
    }

    public void Play()
    {
        animator.SetTrigger("Play");

        StartCoroutine(DestroyAfterAnimation());
    }

    /// <summary>
    /// アニメーションの長さ+マージン後にDestroy
    /// </summary>
    /// <returns></returns>
    private IEnumerator DestroyAfterAnimation()
    {
        // Trigger反映を1F待つ
        yield return null;

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        float destroyTime = stateInfo.length + DestroyMargin;

        Destroy(gameObject, destroyTime);
    }
}