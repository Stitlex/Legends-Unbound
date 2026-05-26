using System;
using UnityEngine;

public class ActiveWeapon : MonoBehaviour
{
    public static ActiveWeapon Instance { get; private set; }

    [SerializeField] private Sword sword;
    [SerializeField] private Bow bow;

    private Vector3 mousePos;
    private Vector3 playerPosition;

    public event EventHandler OnWeaponAttack;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (sword != null) sword.OnSwordSwing += Weapon_OnAttackFired;
        if (bow != null) bow.OnBowAttack += Weapon_OnAttackFired;
    }

    private void Update()
    {
        if (Player.Instance != null && Player.Instance.IsAlive())
        {
            // Оптимізація: отримуємо позиції лише ОДИН раз за кадр
            mousePos = GameInput.Instance.GetMousePosition();
            playerPosition = Player.Instance.GetPlayerScreenPosition();

            FollowMousePosition();
        }
    }
    private void OnDestroy()
    {
        if (sword != null) sword.OnSwordSwing -= Weapon_OnAttackFired;
        if (bow != null) bow.OnBowAttack -= Weapon_OnAttackFired;
    }

    private void Weapon_OnAttackFired(object sender, EventArgs e)
    {
        OnWeaponAttack?.Invoke(this, EventArgs.Empty);
    }

    public void SwitchWeapon(string type)
    {
        sword.gameObject.SetActive(type == "Melee");
        bow.gameObject.SetActive(type == "Ranged");
    }

    public void UpdateWeaponStats(WeaponInfo info)
    {
        if (info.typeWeapon == "Melee")
        {
            sword.UpdateStats(info);
        }
        else if (info.typeWeapon == "Ranged")
        {
            bow.UpdateStats(info);
        }
    }

    public MonoBehaviour GetActiveWeapon()
    {
        if (sword.gameObject.activeSelf)
        {
            return sword;
        }
        else
        {
            return bow;
        }
    }

    public void ExecuteAttack()
    {
        if (Time.timeScale == 0f) return;

        MonoBehaviour activeWeapon = GetActiveWeapon();

        if (activeWeapon is Sword swordComponent)
        {
            swordComponent.Attack();
        }
        else if (activeWeapon is Bow bowComponent)
        {
            bowComponent.Attack();
        }
    }

    public Quaternion GetProjectileRotation()
    {
        Vector2 direction = mousePos - playerPosition;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        return Quaternion.Euler(0, 0, angle);
    }

    private void FollowMousePosition()
    {
        if (mousePos.x < playerPosition.x)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }
}