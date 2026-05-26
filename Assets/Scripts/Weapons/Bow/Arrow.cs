using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class Arrow : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float speed = 15f;
    [SerializeField] private float lifeTime = 5f;
    private int damage;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.TryGetComponent(out Player player))
        {
            return;
        }

        if (collision.TryGetComponent(out EnemyEntity enemy))
        {
            enemy.TakeDamage(damage, "Arrow");
            Destroy(gameObject);
            return;
        }

        if (collision.CompareTag("Environment"))
        {
            Destroy(gameObject);
        }
    }

    public void Setup(int damageAmount)
    {
        this.damage = damageAmount;
        rb.linearVelocity = transform.right * speed;
    }
}