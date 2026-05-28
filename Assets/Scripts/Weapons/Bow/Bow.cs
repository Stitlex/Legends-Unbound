using System;
using UnityEngine;

public class Bow : MonoBehaviour
{
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private Transform arrowSpawnPoint;

    [Header("Settings")]
    [SerializeField] private int damageAmount = 2;
    [SerializeField] private float attackCooldown = 0.8f;

    private float nextAttackTime;

    public event EventHandler OnBowAttack;

    public void Attack()
    {
        if (Time.time < nextAttackTime) return;

        nextAttackTime = Time.time + attackCooldown;
        OnBowAttack?.Invoke(this, EventArgs.Empty);
    }

    public void UpdateStats(WeaponInfo info)
    {
        int playerAgility = Player.Instance != null ? Player.Instance.GetAgility() : 0;
        damageAmount = info.damage + playerAgility;
        attackCooldown = info.attackDuration;
    }

    public void FireArrow()
    {
        Quaternion shootRotation = ActiveWeapon.Instance.GetProjectileRotation();

        GameObject arrowObj = Instantiate(arrowPrefab, arrowSpawnPoint.position, shootRotation);

        if (arrowObj.TryGetComponent(out Arrow arrow))
        {
            arrow.Setup(damageAmount);
        }
    }
}