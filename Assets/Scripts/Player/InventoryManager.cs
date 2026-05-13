using System;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    [Header("Current Equipment")]
    [SerializeField] private WeaponInfo equippedMelee;
    [SerializeField] private WeaponInfo equippedRanged;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        GameInput.Instance.OnSlot1Pressed += GameInput_OnSlot1Pressed;
        GameInput.Instance.OnSlot2Pressed += GameInput_OnSlot2Pressed;

        WeaponInfo sword = DataManager.Instance.GetWeaponInfo("starter_sword");
        WeaponInfo bow = DataManager.Instance.GetWeaponInfo("wooden_bow");

        if (sword != null) equippedMelee = sword;
        if (bow != null) equippedRanged = bow;

        EquipActiveSlot("Melee");
    }

    private void OnDestroy()
    {
        if (GameInput.Instance != null)
        {
            GameInput.Instance.OnSlot1Pressed -= GameInput_OnSlot1Pressed;
            GameInput.Instance.OnSlot2Pressed -= GameInput_OnSlot2Pressed;
        }
    }

    public WeaponInfo GetEquippedMelee() => equippedMelee;
    public WeaponInfo GetEquippedRanged() => equippedRanged;

    private void GameInput_OnSlot1Pressed(object sender, EventArgs e) => EquipActiveSlot("Melee");

    private void GameInput_OnSlot2Pressed(object sender, EventArgs e) => EquipActiveSlot("Ranged");

    private void EquipActiveSlot(string type)
    {
        WeaponInfo weaponToEquip = (type == "Melee") ? equippedMelee : equippedRanged;

        if (weaponToEquip == null || string.IsNullOrEmpty(weaponToEquip.id))
        {
            Debug.LogWarning($"Слот {type} порожній!");
            return;
        }

        ActiveWeapon.Instance.SwitchWeapon(type);
        ActiveWeapon.Instance.UpdateWeaponStats(weaponToEquip);

        Debug.Log($"Одягнено: {weaponToEquip.weaponName}");
    }

    public void ChangeWeapon(string newWeaponID)
    {
        WeaponInfo newInfo = DataManager.Instance.GetWeaponInfo(newWeaponID);

        if (newInfo == null)
        {
            Debug.LogError($"Зброю з ID {newWeaponID} не знайдено!");
            return;
        }

        if (newInfo.typeWeapon == "Melee")
        {
            equippedMelee = newInfo;
            if (ActiveWeapon.Instance.GetActiveWeapon() is Sword)
            {
                EquipActiveSlot("Melee");
            }
        }
        else if (newInfo.typeWeapon == "Ranged")
        {
            equippedRanged = newInfo;
            if (ActiveWeapon.Instance.GetActiveWeapon() is Bow)
            {
                EquipActiveSlot("Ranged");
            }
        }
    }    
}