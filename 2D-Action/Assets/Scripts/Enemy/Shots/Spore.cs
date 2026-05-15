using UnityEngine;
using DG.Tweening;

public class Spore : MonoBehaviour, IBreakable
{
    [SerializeField]
    private int sporeDamage = 1;
    [SerializeField]
    private float lifeTime = 5f;
    [SerializeField]
    private float baseSpeed = 3f;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    private bool isBreaking = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    public void Init(Vector2 direction, float speedMultiplier = 1f)
    {
        float finalSpeed = baseSpeed * speedMultiplier;
        DriftingAnimation();
        rb.linearVelocity = new Vector2(direction.x * finalSpeed, 0f);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // ÉvÉåÉCÉÑÅ[Ç∆è’ìÀ
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();

            if (player != null)
            {
                player.TakeDamage(sporeDamage, transform.position);
            }
            Destroy(gameObject, 0.8f);
            return;
        }

        // ínñ Ç∆è’ìÀ
        if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Destroy(gameObject, 0.8f);
            return;
        }
    }

    public void Break()
    {
        if (isBreaking) return;

        isBreaking = true;

        rb.linearVelocity = Vector2.zero;

        transform.DOKill();

        // è’ìÀéûÇ…çLÇ™Ç¡Çƒè¡Ç¶ÇÈÉAÉjÉÅÅ[ÉVÉáÉì
        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOScale(1.5f, 0.2f));
        seq.Join(spriteRenderer.DOFade(0f, 0.2f));

        seq.OnComplete(() =>
        {
            Destroy(gameObject);
        });
    }

    private void DriftingAnimation()
    {
        // ÉTÉCÉYägèk
        transform.DOScale(1.1f, 0.6f)
                 .SetLoops(-1, LoopType.Yoyo)
                 .SetEase(Ease.InOutSine);

        // è„â∫óhìÆ
        transform.DOMoveY(transform.position.y + 0.15f, 1.2f)
                 .SetLoops(-1, LoopType.Yoyo)
                 .SetEase(Ease.InOutSine);

        // óhÇÍÇÈÇÊÇ§Ç…âÒì]
        transform.DORotate(new Vector3(0, 0, 10f), 1.5f)
                 .SetLoops(-1, LoopType.Yoyo)
                 .SetEase(Ease.InOutSine);
    }
}
