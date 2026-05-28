using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveLoadManager : MonoBehaviour
{
    public static SaveLoadManager Instance { get; private set; }

    private string savesFolder;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            savesFolder = Path.Combine(Application.dataPath, "Saves");
            if (!Directory.Exists(savesFolder)) Directory.CreateDirectory(savesFolder);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        string currentWorld = PlayerPrefs.GetString("SelectedWorld", "DefaultWorld");

        if (Input.GetKeyDown(KeyCode.F5))
        {
            SaveGame(currentWorld);
        }

        if (Input.GetKeyDown(KeyCode.F9))
        {
            LoadGame(currentWorld);
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public bool SaveExists(string worldName)
    {
        string path = Path.Combine(savesFolder, worldName + ".json");
        return File.Exists(path);
    }

    public void SaveGame(string worldName)
    {
        string path = Path.Combine(savesFolder, worldName + ".json");
        GameSaveData saveData = new GameSaveData();

        if (Player.Instance != null)
            Player.Instance.CapturePlayerState(saveData.playerData);

        List<EnemySaveData> enemiesList = new List<EnemySaveData>();
        EnemyEntity[] allEnemies = FindObjectsByType<EnemyEntity>(FindObjectsInactive.Exclude);

        foreach (EnemyEntity enemy in allEnemies)
        {
            if (enemy.IsDead()) continue;

            string configId = enemy.GetEnemyConfigId();

            if (string.IsNullOrEmpty(configId))
            {
                Debug.LogWarning($"Ворога {enemy.name} не має enemyConfigId! Пропускаємо.");
                continue;
            }

            EnemySaveData data = new EnemySaveData
            {
                configId = configId,
                position = new float[] { enemy.transform.position.x, enemy.transform.position.y, enemy.transform.position.z },
                currentHealth = enemy.GetCurrentHealth()
            };
            enemiesList.Add(data);
        }

        saveData.enemiesData = enemiesList;

        string json = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(path, json);
        Debug.Log($"Світ '{worldName}' збережено! Живих ворогів: {enemiesList.Count}");
    }

    public void LoadGame(string worldName)
    {
        string path = Path.Combine(savesFolder, worldName + ".json");
        if (!File.Exists(path))
        {
            Debug.LogWarning($"Файл збереження для світу '{worldName}' не знайдено!");
            return;
        }

        string json = File.ReadAllText(path);
        GameSaveData saveData = JsonUtility.FromJson<GameSaveData>(json);

        if (Player.Instance != null && saveData.playerData != null)
            Player.Instance.LoadPlayerState(saveData.playerData);

        EnemyEntity[] existing = FindObjectsByType<EnemyEntity>(FindObjectsInactive.Exclude);
        foreach (var e in existing)
            Destroy(e.gameObject);

        if (saveData.enemiesData != null && EnemySpawner.Instance != null)
        {
            foreach (var data in saveData.enemiesData)
            {
                EnemySpawner.Instance.SpawnEnemy(data);
            }
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "GameScene")
        {
            if (PlayerPrefs.GetInt("LoadPending", 0) == 1)
            {
                string currentWorld = PlayerPrefs.GetString("SelectedWorld", "DefaultWorld");
                Debug.Log($"[SaveLoadManager] Авто-завантаження світу '{currentWorld}' після зміни сцени.");

                LoadGame(currentWorld);
                PlayerPrefs.SetInt("LoadPending", 0);
            }
        }
    }
}