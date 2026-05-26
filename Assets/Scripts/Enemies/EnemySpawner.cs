using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner Instance { get; private set; }

    [System.Serializable]
    public class EnemyPrefabEntry
    {
        public string configId;
        public GameObject prefab;
    }

    [SerializeField] private List<EnemyPrefabEntry> enemyPrefabs = new List<EnemyPrefabEntry>();

    private Dictionary<string, GameObject> prefabDict = new Dictionary<string, GameObject>();

    private void Awake()
    {
        Instance = this;

        foreach (var entry in enemyPrefabs)
        {
            if (!string.IsNullOrEmpty(entry.configId) && entry.prefab != null)
                prefabDict[entry.configId] = entry.prefab;
        }
    }

    public void SpawnEnemy(EnemySaveData data)
    {
        Debug.Log($"Спроба заспавнити: configId = '{data.configId}'");

        if (!prefabDict.TryGetValue(data.configId, out GameObject prefab))
        {
            Debug.LogError($"Префаб для {data.configId} не знайдено!");
            return;
        }

        GameObject go = Instantiate(prefab,
            new Vector3(data.position[0], data.position[1], data.position[2]), Quaternion.identity);

        EnemyEntity entity = go.GetComponent<EnemyEntity>();
        if (entity != null)
        {
            entity.SetCurrentHealth(data.currentHealth);
        }
    }
}