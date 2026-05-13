using System;
using Unity.VisualScripting;
using UnityEngine;


[RequireComponent(typeof(PolygonCollider2D))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(EnemyAI))]
public class EnemyEntity : MonoBehaviour
{
    [SerializeField] private int maxHealth = 10;
    [SerializeField] private int damageAmount = 1;

    public event EventHandler OnTakeHit;
    public event EventHandler OnDeath;

    private int currentHealth;

    private PolygonCollider2D polygonCollider2D;
    private BoxCollider2D boxCollider2D;
    private EnemyAI enemyAI;

    private void Awake()
    {
        polygonCollider2D = GetComponent<PolygonCollider2D>();
        boxCollider2D = GetComponent<BoxCollider2D>();
        enemyAI = GetComponent<EnemyAI>();
    }

    private void Start()
    {
        currentHealth = maxHealth;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Player player))
        {
            player.TakeDamage(transform, damageAmount);
        }
    }
    public void TakeDamage(int damage, string attackerName = "")
    {
        currentHealth = Mathf.Max(0, currentHealth - damage);
        Debug.Log($"[{attackerName}] вдарив. Шкода: {damage}. Залишене HP: {currentHealth}");
        OnTakeHit?.Invoke(this, EventArgs.Empty);
        DetectDeath();
    }

    public void PolygonColliderTurnOff()
    {
        polygonCollider2D.enabled = false;
    }

    public void PolygonColliderTurnOn()
    {
        polygonCollider2D.enabled = true;
    }

    private void DetectDeath()
    {
        if (currentHealth <= 0)
        {
            polygonCollider2D.enabled = false;
            boxCollider2D.enabled = false;

            enemyAI.SetDeathState();

            OnDeath?.Invoke(this, EventArgs.Empty);
        }
    }
    
}
