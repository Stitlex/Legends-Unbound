using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set; }

    [SerializeField] private TextAsset weaponsJsonFile;
    private Dictionary<string, WeaponInfo> weaponsDatabase = new Dictionary<string, WeaponInfo>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadWeaponData();
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
}