using DG.Tweening;
using UnityEngine;

public class AnimationEffectController : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer spriteRenderer;

    private Tween blinkTween;
    private Tween warningTween;

    private void Awake()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }
    }

    public void KillAllEffects()
    {
        // Tween’Ç‰Á‚²‚Æ‚É’Ç‰Á‚·‚é‚±‚Æ
        blinkTween?.Kill();
        warningTween?.Kill();

        ResetColor();
    }

    private void ResetColor()
    {
        if (spriteRenderer == null) return;

        spriteRenderer.color = Color.white;
    }

    public void PlayInvincibleBlink()
    {
        blinkTween?.Kill();

        blinkTween = spriteRenderer
                     .DOFade(0.3f, 0.08f)
                     .SetLoops(-1, LoopType.Yoyo);
    }

    public void StopInvincibleBlink()
    {
        blinkTween?.Kill();

        Color color = spriteRenderer.color;
        color.a = 1f;
        spriteRenderer.color = color;
    }

    public void PlayHitFlash()
    {
        spriteRenderer.DOColor(Color.white, 0.05f)
                      .SetLoops(2, LoopType.Yoyo);
    }

    public void PlayHitShake()
    {
        transform.DOShakePosition(0.15f, 0.15f);
    }

    public void PlayHitPunch()
    {
        transform.DOPunchScale(Vector3.one * 0.15f, 0.15f);
    }

    public void PlayAttackWarning()
    {
        warningTween?.Kill();

        warningTween = spriteRenderer.DOColor(new Color(1f, 0.5f, 0f) /*ƒIƒŒƒ“ƒW*/ , 0.12f)
                                     .SetLoops(4, LoopType.Yoyo);
    }

    public void StopAttackWarning()
    {
        warningTween?.Kill();

        spriteRenderer.color = Color.white;
    }
}