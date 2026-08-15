using DG.Tweening;
using UnityEngine;

public class AnimationEffectController : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer spriteRenderer;

    private Tween blinkTween;
    private Tween warningTween;
    private Tween hitFlashTween;
    private Tween deathFadeTween;
    private Tween shakeTween;
    private Tween punchTween;

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
        hitFlashTween?.Kill();
        deathFadeTween?.Kill();
        shakeTween?.Kill(true);
        punchTween?.Kill(true);

        ResetColor();
    }

    private void ResetColor()
    {
        if (spriteRenderer == null) return;

        spriteRenderer.color = Color.white;
    }

    public void PlayDeathBlink()
    {
        blinkTween?.Kill();

        blinkTween = spriteRenderer
                     .DOFade(0.3f, 0.2f)
                     .SetLoops(-1, LoopType.Yoyo)
                     .SetLink(gameObject, LinkBehaviour.KillOnDestroy);
    }

    public void PlayDeathFlash()
    {
        hitFlashTween?.Kill();
        ResetColor();

        hitFlashTween = spriteRenderer.DOColor(new Color(1f, 0.5f, 0.5f), 0.3f)
                                      .SetLoops(-1, LoopType.Yoyo)
                                      .SetLink(gameObject, LinkBehaviour.KillOnDestroy)
                                      .OnComplete(ResetColor);
    }

    public void PlayInvincibleBlink()
    {
        blinkTween?.Kill();

        blinkTween = spriteRenderer
                     .DOFade(0.3f, 0.08f)
                     .SetLoops(-1, LoopType.Yoyo)
                     .SetLink(gameObject, LinkBehaviour.KillOnDestroy);
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
        hitFlashTween?.Kill();
        ResetColor();

        hitFlashTween = spriteRenderer.DOColor(new Color(1f, 0.5f, 0.5f), 0.1f)
                                      .SetLoops(4, LoopType.Yoyo)
                                      .SetLink(gameObject, LinkBehaviour.KillOnDestroy)
                                      .OnComplete(ResetColor);
    }

    public void PlayHitShake()
    {
        shakeTween?.Kill(true);

        shakeTween = transform.DOShakePosition(0.2f, 0.2f)
                              .SetLink(gameObject, LinkBehaviour.KillOnDestroy);
    }

    public void PlayHitPunch()
    {
        punchTween?.Kill(true);

        punchTween = transform.DOPunchScale(Vector3.one * 0.15f, 0.15f)
                              .SetLink(gameObject, LinkBehaviour.KillOnDestroy);
    }

    public void PlayDeathFade(float delaySeconds, float durationSeconds, System.Action onComplete)
    {
        deathFadeTween?.Kill();

        deathFadeTween = spriteRenderer
                         .DOFade(0f, Mathf.Max(0f, durationSeconds))
                         .SetDelay(Mathf.Max(0f, delaySeconds))
                         .SetLink(gameObject, LinkBehaviour.KillOnDestroy)
                         .OnComplete(() => onComplete?.Invoke());
    }

    public void PlayAttackWarning()
    {
        warningTween?.Kill();

        warningTween = spriteRenderer.DOColor(new Color(1f, 0.5f, 0f) /*ƒIƒŒƒ“ƒW*/ , 0.12f)
                                     .SetLoops(4, LoopType.Yoyo)
                                     .SetLink(gameObject, LinkBehaviour.KillOnDestroy);
    }

    public void StopAttackWarning()
    {
        warningTween?.Kill();

        spriteRenderer.color = Color.white;
    }
}