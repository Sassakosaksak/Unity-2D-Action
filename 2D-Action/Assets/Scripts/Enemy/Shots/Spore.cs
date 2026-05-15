using UnityEngine;

public class Spore : MonoBehaviour, IBreakable
{
    [SerializeField]
    private int sporeDamage = 1;
    [SerializeField]
    private float lifeTime = 5f;
    [SerializeField]
    private float baseSpeed = 3f;

    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    public void Init(Vector2 direction, float speedMultiplier = 1f)
    {
        float finalSpeed = baseSpeed * speedMultiplier;
        rb.linearVelocity = new Vector2(direction.x * finalSpeed, 0f);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // プレイヤーと衝突
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();

            if (player != null)
            {
                player.TakeDamage(sporeDamage, transform.position);
            }
            Destroy(gameObject);
            return;
        }

        // 地面と衝突
        if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Destroy(gameObject);
            return;
        }
    }

    public void Break()
    {
        Destroy(gameObject);
    }
}
