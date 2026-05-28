using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    [Header("Current Equipment")]
    [SerializeField] private WeaponInfo equippedMelee;
    [SerializeField] private WeaponInfo equippedRanged;

    [Header("Inventory Data")]
    [SerializeField] private int gold = 0;
    [SerializeField] private List<string> unlockedWeaponIds = new List<string>();
    [SerializeField] private List<string> unlockedFoodIds = new List<string>();

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (unlockedWeaponIds.Count == 0)
        {
            AddWeaponToUnlocked("starter_sword");
            AddWeaponToUnlocked("wooden_bow");

            WeaponInfo sword = DataManager.Instance.GetWeaponInfo("starter_sword");
            WeaponInfo bow = DataManager.Instance.GetWeaponInfo("wooden_bow");

            if (sword != null) equippedMelee = sword;
            if (bow != null) equippedRanged = bow;
        }

        if (GameInput.Instance != null)
        {
            GameInput.Instance.OnSlot1Pressed += GameInput_OnSlot1Pressed;
            GameInput.Instance.OnSlot2Pressed += GameInput_OnSlot2Pressed;
        }

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

    
    public int GetGold() => gold;
    public List<string> GetUnlockedWeaponIds() => unlockedWeaponIds;

    public WeaponInfo GetEquippedMelee() => equippedMelee;
    public WeaponInfo GetEquippedRanged() => equippedRanged;

    public List<string> GetUnlockedFoodIds() => unlockedFoodIds;

    public void AddFoodToInventory(string foodId)
    {
        unlockedFoodIds.Add(foodId);
    }

    public bool IsWeaponEquipped(string weaponId)
    {
        if (equippedMelee != null && equippedMelee.id == weaponId) return true;
        if (equippedRanged != null && equippedRanged.id == weaponId) return true;
        return false;
    }

    private void GameInput_OnSlot1Pressed(object sender, EventArgs e) => EquipActiveSlot("Melee");

    private void GameInput_OnSlot2Pressed(object sender, EventArgs e) => EquipActiveSlot("Ranged");

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
        }
        else if (newInfo.typeWeapon == "Ranged")
        {
            equippedRanged = newInfo;
        }
    }

    public void RefreshActiveWeapon()
    {
        MonoBehaviour active = ActiveWeapon.Instance.GetActiveWeapon();

        if (active is Sword)
        {
            EquipActiveSlot("Melee");
        }
        else if (active is Bow)
        {
            EquipActiveSlot("Ranged");
        }
    }

    public void AddWeaponToUnlocked(string weaponId)
    {
        if (!unlockedWeaponIds.Contains(weaponId))
        {
            unlockedWeaponIds.Add(weaponId);
            Debug.Log($"[КОЛЕКЦІЯ] Нову зброю [{weaponId}] додано до вашого арсеналу!");
        }
        else
        {
            Debug.Log($"[КОЛЕКЦІЯ] Зброя [{weaponId}] вже є у вашому списку.");
        }
    }

    public void SetUnlockedWeapons(List<string> weaponIds)
    {
        if (weaponIds != null)
        {
            unlockedWeaponIds = new List<string>(weaponIds);
        }
    }

    public void AddGold(int amount)
    {
        gold += amount;
        Debug.Log($"[ГАМАНЕЦЬ] Отримано золота: +{amount}. Баланс: {gold} монет.");
    }

    public void SetGold(int amount)
    {
        gold = amount;
    }

    public void UseFood(string foodId)
    {
        ItemInfo food = DataManager.Instance.GetFoodInfo(foodId);
        if (food == null) return;

        if (Player.Instance != null)
            Player.Instance.Heal(food.healthRestore);

        unlockedFoodIds.Remove(foodId);
    }

    public void SetUnlockedFood(List<string> foodIds)
    {
        if (foodIds != null)
        {
            unlockedFoodIds = new List<string>(foodIds);
        }
    }

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
}