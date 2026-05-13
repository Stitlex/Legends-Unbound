using System;
using UnityEngine;

public class Sword : MonoBehaviour
{
    private int damageAmount;
    private float attackCooldown;

    private float damageCooldown = 0.1f;
    private float lastDamageTime = 2;
    private float nextAttackTime = 0.5f;


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
        damageAmount = info.damage;
        attackCooldown = info.attackDuration;
    }

    public void AttackColliderTurnOff() => polygonCollider2D.enabled = false;
    public void AttackColliderTurnOn() => polygonCollider2D.enabled = true;
}
