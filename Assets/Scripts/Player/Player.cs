using System;
using UnityEngine;

[SelectionBase]
public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }
    public event EventHandler OnPlayerDeath;

    [Header("Movement")]
    [SerializeField] private float movingSpeed = 5f;
    [SerializeField] private float minMovingSpeed = 0.1f;

    [Header("Health & Combat")]
    [SerializeField] private int maxHealth = 10;
    [SerializeField] private float damageReloadTime = 0.5f;

    Vector2 inputVector;
    private Rigidbody2D rb;
    private int currentHealth;

    private bool canTakeDamage;
    private bool isAlive;
    private bool isRunning = false;


    private void Awake()
    {
        Instance = this;
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        currentHealth = maxHealth;
        canTakeDamage = true;
        isAlive = true;
        GameInput.Instance.OnPlayerAttack += Player_OnPlayerAttack;
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

    public bool IsAlive() => isAlive;
    public bool IsRunning() => isRunning;

    public Vector3 GetPlayerScreenPosition()
    {
        Vector3 playerScreenPosition = Camera.main.WorldToScreenPoint(transform.position);
        return playerScreenPosition;
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
        }
    }

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

    private void Player_OnPlayerAttack(object sender, System.EventArgs e)
    {
        ActiveWeapon.Instance.ExecuteAttack();
    }
}