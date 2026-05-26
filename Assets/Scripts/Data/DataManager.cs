using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set; }

    [SerializeField] private TextAsset weaponsJsonFile;
    private Dictionary<string, WeaponInfo> weaponsDatabase = new Dictionary<string, WeaponInfo>();

    [SerializeField] private TextAsset enemiesJsonFile;
    private Dictionary<string, EnemyInfo> enemiesDatabase = new Dictionary<string, EnemyInfo>();

    [SerializeField] private TextAsset foodJsonFile;
    private Dictionary<string, ItemInfo> foodDatabase = new Dictionary<string, ItemInfo>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadWeaponData();
            LoadEnemyData();
            LoadFoodData();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public WeaponInfo GetWeaponInfo(string id)
    {
        if (weaponsDatabase.TryGetValue(id, out WeaponInfo info)) return info;
        return null;
    }

    private void LoadWeaponData()
    {
        if (weaponsJsonFile != null)
        {
            WeaponDataContainer data = JsonUtility.FromJson<WeaponDataContainer>(weaponsJsonFile.text);

            foreach (var weapon in data.weapons)
            {
                weaponsDatabase[weapon.id] = weapon;
                Debug.Log($"Завантажено {weapon.weaponName} з ID {weapon.id}");
            }
        }
    }

    public EnemyInfo GetEnemyInfo(string id)
    {
        if (enemiesDatabase.TryGetValue(id, out EnemyInfo info)) return info;
        Debug.LogError($"Ворога з ID {id} не знайдено в базі даних!");
        return null;
    }

    private void LoadEnemyData()
    {
        if (enemiesJsonFile != null)
        {
            EnemyDataContainer data = JsonUtility.FromJson<EnemyDataContainer>(enemiesJsonFile.text);

            foreach (var enemy in data.enemies)
            {
                enemiesDatabase[enemy.enemyId] = enemy;
            }
        }
    }

    public ItemInfo GetFoodInfo(string id)
    {
        if (foodDatabase.TryGetValue(id, out ItemInfo info)) return info;
        Debug.LogError($"Предмет їжі з ID {id} не знайдено в базі даних!");
        return null;
    }

    private void LoadFoodData()
    {
        if (foodJsonFile != null)
        {
            ItemDataContainer data = JsonUtility.FromJson<ItemDataContainer>(foodJsonFile.text);

            foreach (var item in data.items)
            {
                foodDatabase[item.id] = item;
                Debug.Log($"Завантажено предмет: {item.itemName} з ID {item.id}");
            }
        }
    }
}