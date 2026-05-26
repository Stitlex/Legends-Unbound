using System;
using UnityEngine;

[RequireComponent(typeof(PolygonCollider2D))]
public class Sword : MonoBehaviour
{
    private int damageAmount;
    private float attackCooldown;

    private float damageCooldown = 0f;
    private float lastDamageTime = 0f;
    private float nextAttackTime = 0f;


    public event EventHandler OnSwordSwing;

    private PolygonCollider2D polygonCollider2D;

    private void Awake()
    {
        polygonCollider2D = GetComponent<PolygonCollider2D>();
    }

    private void Start()
    {
        AttackColliderTurnOff();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (Time.time > lastDamageTime + damageCooldown)
        {
            if (collision.transform.TryGetComponent(out EnemyEntity enemyEntity))
            {
                enemyEntity.TakeDamage(damageAmount, transform.root.name);
                lastDamageTime = Time.time;
            }
        }
    }

    public void Attack()
    {
        if (Time.time < nextAttackTime) return;

        nextAttackTime = Time.time + attackCooldown;

        OnSwordSwing?.Invoke(this, EventArgs.Empty);
    }

    public void UpdateStats(WeaponInfo info)
    {
        int playerStrength = Player.Instance != null ? Player.Instance.GetStrength() : 0;
        damageAmount = info.damage + playerStrength;

        attackCooldown = info.attackDuration;
    }

    public void AttackColliderTurnOff() => polygonCollider2D.enabled = false;
    public void AttackColliderTurnOn() => polygonCollider2D.enabled = true;
}
