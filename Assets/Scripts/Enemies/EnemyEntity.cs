using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PolygonCollider2D))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(EnemyAI))]
public class EnemyEntity : MonoBehaviour
{
    [Header("Database Config")]
    [SerializeField] private string enemyConfigId;

    private int maxHealth;
    private int damageAmount;
    private int expReward;
    private List<LootInfo> lootTable;
    private int minGold;
    private int maxGold;

    public event EventHandler OnTakeHit;
    public event EventHandler OnDeath;

    private int currentHealth;

    private PolygonCollider2D polygonCollider2D;
    private BoxCollider2D boxCollider2D;
    private EnemyAI enemyAI;

    private bool isHealthLoadedFromSave = false;

    private void Awake()
    {
        polygonCollider2D = GetComponent<PolygonCollider2D>();
        boxCollider2D = GetComponent<BoxCollider2D>();
        enemyAI = GetComponent<EnemyAI>();
    }

    private void Start()
    {
        LoadConfig();
        if (!isHealthLoadedFromSave)
        {
            currentHealth = maxHealth;
        }
    }

    private void LoadConfig()
    {
        if (string.IsNullOrEmpty(enemyConfigId))
        {
            Debug.LogError($"[ПОМИЛКА] На об'єкті {gameObject.name} не вказано Enemy Config Id!");
            SetDefaultStats();
            return;
        }

        EnemyInfo config = DataManager.Instance.GetEnemyInfo(enemyConfigId);
        if (config != null)
        {
            maxHealth = config.baseHealth;
            damageAmount = config.damage;
            expReward = config.expReward;
            lootTable = config.lootTable;
            minGold = config.minGoldReward;
            maxGold = config.maxGoldReward;

            var navAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
            if (navAgent != null) navAgent.speed = config.speed;
        }
        else
        {
            Debug.LogWarning($"Конфіг для '{enemyConfigId}' не знайдено. Використовуємо дефолт.");
            SetDefaultStats();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Player player))
        {
            if (polygonCollider2D != null && collision.IsTouching(polygonCollider2D))
            {
                player.TakeDamage(transform, damageAmount);
            }
        }
    }

    public int GetCurrentHealth() => currentHealth;
    public bool IsDead() => currentHealth <= 0;
    public string GetEnemyConfigId() => enemyConfigId;

    public void SetCurrentHealth(int health)
    {
        currentHealth = health;
        isHealthLoadedFromSave = true;
    }

    public void TakeDamage(int damage, string attackerName = "")
    {
        if (currentHealth <= 0) return;

        currentHealth = Mathf.Max(0, currentHealth - damage);
        Debug.Log($"[{attackerName}] вдарив. Шкода: {damage}. Залишене HP: {currentHealth}");

        OnTakeHit?.Invoke(this, EventArgs.Empty);

        if (currentHealth <= 0)
        {
            if (Player.Instance != null)
                Player.Instance.AddExperience(expReward);

            DetectDeath();
        }
    }

    private void DetectDeath()
    {
        if (currentHealth <= 0)
        {
            polygonCollider2D.enabled = false;
            boxCollider2D.enabled = false;
            enemyAI.SetDeathState();

            DropLoot();
            OnDeath?.Invoke(this, EventArgs.Empty);

            Invoke(nameof(DestroySelf), 0.7f);
        }
    }

    private void DestroySelf()
    {
        Destroy(gameObject);
    }

    public void PolygonColliderTurnOff() => polygonCollider2D.enabled = false;
    public void PolygonColliderTurnOn() => polygonCollider2D.enabled = true;

    private void DropLoot()
    {
        int randomGold = UnityEngine.Random.Range(minGold, maxGold + 1);
        if (InventoryManager.Instance != null)
            InventoryManager.Instance.AddGold(randomGold);

        if (lootTable == null || lootTable.Count == 0) return;

        foreach (var loot in lootTable)
        {
            if (UnityEngine.Random.value <= loot.dropChance)
            {
                InventoryManager.Instance.AddWeaponToUnlocked(loot.weaponId);
                break;
            }
        }
    }

    private void SetDefaultStats()
    {
        maxHealth = 10; damageAmount = 2; expReward = 20;
        minGold = 5; maxGold = 10;
        lootTable = new List<LootInfo>();
    }
}