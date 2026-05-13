using UnityEngine;

public class ActiveWeapon : MonoBehaviour
{
    public static ActiveWeapon Instance { get; private set; }

    [SerializeField] private Sword sword;
    [SerializeField] private Bow bow;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (Player.Instance.IsAlive()) FollowMousePosition();
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
        Vector3 mousePos = GameInput.Instance.GetMousePosition();
        Vector3 playerPosition = Player.Instance.GetPlayerScreenPosition();
        Vector2 direction = mousePos - playerPosition;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        return Quaternion.Euler(0, 0, angle);
    }

    private void FollowMousePosition()
    {
        Vector3 mousePos = GameInput.Instance.GetMousePosition();
        Vector3 playerPosition = Player.Instance.GetPlayerScreenPosition();

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