using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Панелі інтерфейсу (UI Panels)")]
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject newGamePanel;
    [SerializeField] private GameObject loadGamePanel;

    [Header("Кнопки головного екрану")]
    [SerializeField] private Button openNewGameMenuButton;
    [SerializeField] private Button openLoadGameMenuButton;
    [SerializeField] private Button quitGameButton;

    [Header("Елементи панелі Нової Гри")]
    [SerializeField] private TMP_InputField worldNameInput;
    [SerializeField] private Button startNewWorldButton;
    [SerializeField] private Button backFromNewGameButton;

    [Header("Елементи панелі Завантаження")]
    [SerializeField] private Button backFromLoadGameButton;
    [SerializeField] private GameObject saveSlotPrefab;
    [SerializeField] private Transform listContainer;

    private void Start()
    {
        ShowMainPanel();

        if (openNewGameMenuButton != null) openNewGameMenuButton.onClick.AddListener(ShowNewGamePanel);
        if (openLoadGameMenuButton != null) openLoadGameMenuButton.onClick.AddListener(ShowLoadGamePanel);
        if (quitGameButton != null) quitGameButton.onClick.AddListener(QuitGame);

        if (startNewWorldButton != null) startNewWorldButton.onClick.AddListener(CreateNewWorld);
        if (backFromNewGameButton != null) backFromNewGameButton.onClick.AddListener(ShowMainPanel);

        if (backFromLoadGameButton != null) backFromLoadGameButton.onClick.AddListener(ShowMainPanel);
    }

    public void ShowMainPanel() => SetPanel(mainPanel);
    public void ShowNewGamePanel() => SetPanel(newGamePanel);
    public void ShowLoadGamePanel()
    {
        SetPanel(loadGamePanel);
        RefreshSaveList();
    }

    public void RefreshSaveList()
    {
        if (listContainer == null || saveSlotPrefab == null) return;

        foreach (Transform child in listContainer)
        {
            Destroy(child.gameObject);
        }

        string path = Path.Combine(Application.dataPath, "Saves");
        if (!Directory.Exists(path)) return;

        string[] files = Directory.GetFiles(path, "*.json");

        foreach (string file in files)
        {
            string fileName = Path.GetFileNameWithoutExtension(file);
            GameObject slotObj = Instantiate(saveSlotPrefab, listContainer);

            SaveSlotUI slotUI = slotObj.GetComponent<SaveSlotUI>();
            if (slotUI != null)
            {
                slotUI.Setup(fileName, PerformLoad, PerformDelete);
            }
        }
    }

    private void SetPanel(GameObject activePanel)
    {
        if (mainPanel != null) mainPanel.SetActive(activePanel == mainPanel);
        if (newGamePanel != null) newGamePanel.SetActive(activePanel == newGamePanel);
        if (loadGamePanel != null) loadGamePanel.SetActive(activePanel == loadGamePanel);
    }

    private void CreateNewWorld()
    {
        if (worldNameInput == null) return;

        string name = worldNameInput.text.Trim();
        if (string.IsNullOrWhiteSpace(name))
        {
            Debug.LogWarning("Назва світу порожня!");
            return;
        }

        if (SaveLoadManager.Instance != null && SaveLoadManager.Instance.SaveExists(name))
        {
            Debug.LogWarning($"Світ із назвою '{name}' вже існує! Оберіть інше ім'я.");
            return;
        }

        PlayerPrefs.SetString("SelectedWorld", name);

        SceneManager.LoadScene("GameScene");
    }

    private void QuitGame()
    {
        Debug.Log("Вихід з гри...");
        Application.Quit();
    }

    private void PerformLoad(string fileName)
    {
        Debug.Log($"Вибрано збереження для завантаження: {fileName}");

        PlayerPrefs.SetInt("LoadPending", 1);

        PlayerPrefs.SetString("SelectedWorld", fileName);

        SceneManager.LoadScene("GameScene");
    }

    private void PerformDelete(string fileName)
    {
        string filePath = Path.Combine(Application.dataPath, "Saves", fileName + ".json");

        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            Debug.Log($"Файл збереження '{fileName}.json' успішно видалено.");
            RefreshSaveList();
        }
    }
}