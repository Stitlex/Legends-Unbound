using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [Header("Prefabs & Containers")]
    [SerializeField] private GameObject itemRowPrefab;
    [SerializeField] private Transform listContentContainer;

    [Header("Details Panel")]
    [SerializeField] private GameObject detailsPanel;
    [SerializeField] private TextMeshProUGUI detailNameText;
    [SerializeField] private TextMeshProUGUI detailDescriptionText;
    [SerializeField] private TextMeshProUGUI detailStatText;
    [SerializeField] private Button equipButton;

    [SerializeField] private Button closeDetailsButton;

    private string lastSelectedId = "";

    private string currentCategory = "All";


    private void Start()
    {
        detailsPanel.SetActive(false);

        if (closeDetailsButton != null)
            closeDetailsButton.onClick.AddListener(CloseDetailsPanel);
    }

    public void UpdateInventoryList()
    {
        foreach (Transform child in listContentContainer)
            Destroy(child.gameObject);

        if (InventoryManager.Instance == null) return;

        WeaponInfo currentMelee = InventoryManager.Instance.GetEquippedMelee();
        WeaponInfo currentRanged = InventoryManager.Instance.GetEquippedRanged();

        if (currentCategory == "All" || currentCategory == "Weapons")
        {
            Dictionary<string, int> weaponCounts = new Dictionary<string, int>();
            foreach (string weaponId in InventoryManager.Instance.GetUnlockedWeaponIds())
            {
                if (weaponCounts.ContainsKey(weaponId))
                    weaponCounts[weaponId]++;
                else
                    weaponCounts[weaponId] = 1;
            }

            foreach (var pair in weaponCounts)
            {
                string weaponId = pair.Key;
                int count = pair.Value;

                WeaponInfo info = DataManager.Instance.GetWeaponInfo(weaponId);
                if (info != null)
                {
                    bool isEquipped = (currentMelee != null && currentMelee.id == weaponId) ||
                                      (currentRanged != null && currentRanged.id == weaponId);

                    CreateItemRow(info.id, info.weaponName, "Weapon", isEquipped, count);
                }
            }
        }

        if (currentCategory == "All" || currentCategory == "Food")
        {
            Dictionary<string, int> foodCounts = new Dictionary<string, int>();
            List<string> foodItems = InventoryManager.Instance.GetUnlockedFoodIds();
            foreach (string foodId in foodItems)
            {
                if (foodCounts.ContainsKey(foodId))
                    foodCounts[foodId]++;
                else
                    foodCounts[foodId] = 1;
            }

            foreach (var pair in foodCounts)
            {
                string foodId = pair.Key;
                int count = pair.Value;

                ItemInfo info = DataManager.Instance.GetFoodInfo(foodId);
                if (info != null)
                {
                    CreateItemRow(info.id, info.itemName, "Food", false, count);
                }
            }
        }
    }

    private void CreateItemRow(string id, string name, string type, bool isEquipped, int count)
    {
        GameObject row = Instantiate(itemRowPrefab, listContentContainer);
        if (row.TryGetComponent(out InventoryItemUI itemUI))
        {
            itemUI.Setup(id, name, type, count, ShowItemDetails);
            itemUI.SetEquippedVisual(isEquipped);
        }
    }

    private void ShowItemDetails(string id, string type)
    {
        if (lastSelectedId == id && detailsPanel.activeSelf)
        {
            CloseDetailsPanel();
            lastSelectedId = "";
            return;
        }

        lastSelectedId = id;
        detailsPanel.SetActive(true);

        if (equipButton != null)
        {
            equipButton.onClick.RemoveAllListeners();
            equipButton.gameObject.SetActive(false);
        }

        if (type == "Weapon")
        {
            WeaponInfo info = DataManager.Instance.GetWeaponInfo(id);
            if (info != null)
            {
                detailNameText.text = info.weaponName;
                detailDescriptionText.text = $"Тип: {info.typeWeapon}";
                detailStatText.text = $"Шкода: {info.damage}";

                if (equipButton != null)
                {
                    equipButton.gameObject.SetActive(true);
                    equipButton.onClick.AddListener(() => {
                        InventoryManager.Instance.ChangeWeapon(id);

                        InventoryManager.Instance.RefreshActiveWeapon();

                        Debug.Log($"Зброю {info.weaponName} успішно одягнено!");

                        UpdateInventoryList();
                    });
                }
            }
        }
        else if (type == "Food")
        {
            ItemInfo info = DataManager.Instance.GetFoodInfo(id);
            if (info != null)
            {
                detailNameText.text = info.itemName;
                detailDescriptionText.text = info.description;
                detailStatText.text = $"+{info.healthRestore} HP";

                if (equipButton != null)
                {
                    equipButton.gameObject.SetActive(true);

                    equipButton.onClick.AddListener(() => {
                        InventoryManager.Instance.UseFood(id);
                        Debug.Log($"Ви з'їли {info.itemName}!");

                        CloseDetailsPanel();

                        UpdateInventoryList();
                    });
                }
            }
        }
    }

    public void SelectCategory(string category)
    {
        currentCategory = category;
        UpdateInventoryList();
    }

    public bool IsDetailsPanelOpen()
    {
        return detailsPanel != null && detailsPanel.activeSelf;
    }

    public void CloseDetailsPanel()
    {
        if (detailsPanel != null)
            detailsPanel.SetActive(false);

        lastSelectedId = "";
    }
}