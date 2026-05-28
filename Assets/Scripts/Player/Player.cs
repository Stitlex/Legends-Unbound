using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;


[SelectionBase]
[RequireComponent(typeof(CapsuleCollider2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }
    public event EventHandler OnPlayerDeath;
    public event EventHandler OnLevelUp;

    [Header("Movement")]
    [SerializeField] private float movingSpeed = 5f;
    [SerializeField] private float minMovingSpeed = 0.1f;
    [SerializeField] private int dashSpeed = 5;
    [SerializeField] private float dashTime = 0.2f;
    [SerializeField] private float dashCoolDownTime = 1f;
    [SerializeField] private TrailRenderer trailRenderer;

    [Header("Health & Combat")]
    [SerializeField] private int maxHealth = 10;
    [SerializeField] private float damageReloadTime = 0.5f;

    [Header("RPG Progression")]
    [SerializeField] private int level = 1;
    [SerializeField] private int skillPoints = 0;
    [SerializeField] private int currentExp = 0;
    [SerializeField] private int expToNextLevel = 100;

    [SerializeField] private int strength = 5;
    [SerializeField] private int agility = 5;

    Vector2 inputVector;
    private Rigidbody2D rb;

    private int currentHealth;
    private bool canTakeDamage;
    private bool isAlive;
    private bool isRunning = false;
    private float initialMovingSpeed;
    private bool isDashing;


    private void Awake()
    {
        Instance = this;
        rb = GetComponent<Rigidbody2D>();
        initialMovingSpeed = movingSpeed;
    }

    private void Start()
    {
        currentHealth = maxHealth;
        canTakeDamage = true;
        isAlive = true;
        GameInput.Instance.OnPlayerAttack += PlayerOnPlayerAttack;
        GameInput.Instance.OnPlayerDash += PlayerOnPlayerDash;
    }

    private void Update()
    {
        inputVector = GameInput.Instance.GetMovementVector();
    }

    private void FixedUpdate()
    {
        if (!isAlive) return;
        HandleMovement();
    }

    private void HandleMovement()
    {
        rb.MovePosition(rb.position + inputVector * (movingSpeed * Time.fixedDeltaTime));
        if (Mathf.Abs(inputVector.x) > minMovingSpeed || Mathf.Abs(inputVector.y) > minMovingSpeed)
        {
            isRunning = true;
        }
        else
        {
            isRunning = false;
        }
    }

    public bool IsAlive() => isAlive;
    public bool IsRunning() => isRunning;

    public int GetStrength() => strength;
    public int GetAgility() => agility;
    public int GetCurrentHealth() => currentHealth;
    public int GetMaxHealth() => maxHealth;
    public int GetLevel() => level;
    public int GetSkillPoints() => skillPoints;
    public int GetCurrentExp() => currentExp;
    public int GetExpToNextLevel() => expToNextLevel;
    public Vector3 GetPlayerScreenPosition() => Camera.main.WorldToScreenPoint(transform.position);

    public void TakeDamage(Transform damageSource, int damage)
    {
        if (canTakeDamage && isAlive)
        {
            canTakeDamage = false;
            currentHealth = Mathf.Max(0, currentHealth - damage);

            string attackerName = (damageSource != null) ? damageSource.name : "Unknown Enemy";
            Debug.Log($"[{attackerName}] Damage: {damage}. Health left: {currentHealth}");

            Invoke(nameof(ResetDamageCooldown), damageReloadTime);

            if (currentHealth <= 0)
            {
                Debug.Log("Player Died!");
                DetectDeath();
            }
        }
    }

    private void ResetDamageCooldown()
    {
        canTakeDamage = true;
    }

    private void DetectDeath()
    {
        if (currentHealth == 0 && isAlive)
        {
            isAlive = false;

            GameInput.Instance.DisableMovement();

            OnPlayerDeath?.Invoke(this, EventArgs.Empty);

            Invoke(nameof(GoToMainMenuAfterDeath), 1.5f);
        }
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        Debug.Log($"Гравця вилікувано на +{amount} HP");
    }

    public void AddExperience(int amount)
    {
        if (!isAlive) return;

        currentExp += amount;
        Debug.Log($"Отримано досвід: +{amount}. Поточний досвід: {currentExp}/{expToNextLevel}");

        while (currentExp >= expToNextLevel)
        {
            LevelUp();
        }
    }

    private void LevelUp()
    {
        currentExp -= expToNextLevel;
        level++;

        expToNextLevel = Mathf.RoundToInt(expToNextLevel * 1.2f);

        //maxHealth += 2;
        //currentHealth = maxHealth;
        //strength += 2;
        //agility += 2;

        AddSkillPoints(3);

        Debug.Log($"LEVEL UP! Новий рівень: {level}. Отримано 3 очки прокачки");
        OnLevelUp?.Invoke(this, EventArgs.Empty);
    }

    public bool SpendSkillPoint(string stat)
    {
        if (skillPoints <= 0) return false;

        switch (stat.ToLower())
        {
            case "hp":
            case "health":
                maxHealth += 5;
                currentHealth += 5;
                Debug.Log($"+5 Максимального HP. Тепер: {maxHealth}");
                break;

            case "strength":
                strength += 2;
                Debug.Log($"+3 Сили. Тепер: {strength}");
                break;

            case "agility":
                agility += 2;
                Debug.Log($"+3 Спритності. Тепер: {agility}");
                break;

            default:
                Debug.LogWarning($"Невідома характеристика: {stat}");
                return false;
        }

        skillPoints--;
        Debug.Log($"Прокачено {stat}. Залишилось очок: {skillPoints}");
        return true;
    }

    public void AddSkillPoints(int amount)
    {
        skillPoints += amount;
        Debug.Log($"Отримано {amount} очок прокачки! Всього: {skillPoints}");
    }

    public void CapturePlayerState(PlayerSaveData data)
    {
        data.position = new float[] { transform.position.x, transform.position.y, transform.position.z };
        data.currentHealth = currentHealth;

        // Записуємо нові стати
        data.level = level;
        data.skillPoints = skillPoints;
        data.currentExp = currentExp;
        data.expToNextLevel = expToNextLevel;
        data.strength = strength;
        data.agility = agility;

        if (InventoryManager.Instance != null)
        {
            data.gold = InventoryManager.Instance.GetGold();
            data.unlockedWeapons = InventoryManager.Instance.GetUnlockedWeaponIds();
            data.unlockedFood = InventoryManager.Instance.GetUnlockedFoodIds();

            var melee = InventoryManager.Instance.GetEquippedMelee();
            var ranged = InventoryManager.Instance.GetEquippedRanged();

            data.equippedMeleeId = melee != null ? melee.id : "";
            data.equippedRangedId = ranged != null ? ranged.id : "";
        }
    }

    public void LoadPlayerState(PlayerSaveData data)
    {
        if (data.position != null && data.position.Length == 3)
        {
            transform.position = new Vector3(data.position[0], data.position[1], data.position[2]);
            if (rb != null) rb.position = transform.position;
        }

        currentHealth = data.currentHealth;
        if (currentHealth > 0) isAlive = true;

        level = data.level;
        skillPoints = data.skillPoints;
        currentExp = data.currentExp;
        expToNextLevel = data.expToNextLevel;
        strength = data.strength;
        agility = data.agility;

        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.SetGold(data.gold);

            if (data.unlockedWeapons != null)
            {
                InventoryManager.Instance.SetUnlockedWeapons(data.unlockedWeapons);
            }

            if (data.unlockedFood != null)
            {
                InventoryManager.Instance.SetUnlockedFood(data.unlockedFood);
            }

            if (!string.IsNullOrEmpty(data.equippedMeleeId))
            {
                InventoryManager.Instance.ChangeWeapon(data.equippedMeleeId);
            }

            if (!string.IsNullOrEmpty(data.equippedRangedId))
            {
                InventoryManager.Instance.ChangeWeapon(data.equippedRangedId);
            }

            InventoryManager.Instance.RefreshActiveWeapon();
        }
    }
    private void PlayerOnPlayerAttack(object sender, System.EventArgs e)
    {
        ActiveWeapon.Instance.ExecuteAttack();
    }

    private void PlayerOnPlayerDash(object sender, System.EventArgs e)
    {
        if (isDashing) return;

        Dash();
    }

    private void Dash()
    {
        StartCoroutine(DashRountine());
    }

    private IEnumerator DashRountine()
    {
        isDashing = true;
        movingSpeed = initialMovingSpeed * dashSpeed;
        trailRenderer.emitting = true;
        yield return new WaitForSeconds(dashTime);

        trailRenderer.emitting = false;
        movingSpeed = initialMovingSpeed;

        yield return new WaitForSeconds(dashCoolDownTime);
        isDashing = false;
    }

    private void GoToMainMenuAfterDeath()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}