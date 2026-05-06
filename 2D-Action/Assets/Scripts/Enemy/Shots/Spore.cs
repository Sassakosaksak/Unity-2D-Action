using UnityEngine;

public class Spore : MonoBehaviour
{
    public int damage = 1;
    public float baseSpeed = 3f;

    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Init(Vector2 direction, float speedMultiplier = 1f)
    {
        float finalSpeed = baseSpeed * speedMultiplier;
        rb.linearVelocity = direction * finalSpeed;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // プレイヤーと衝突
        if (other.CompareTag("Player"))
        {
            // TODO：ダメージ処理

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
}
