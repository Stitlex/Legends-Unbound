using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // ОБОВ'ЯЗКОВО для роботи з компонентом Button

public class PauseMenuManager : MonoBehaviour
{
    public static PauseMenuManager Instance { get; private set; }

    [Header("UI Elements")]
    [SerializeField] private GameObject pauseMenuPanel;

    // Додаємо поля для самих кнопок, які з'являться в інспекторі
    [Header("Menu Buttons")]
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button saveButton;
    [SerializeField] private Button exitButton;

    private bool isPaused = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        Resume();

        // Підписуємось на подію натискання Esc з GameInput
        GameInput.Instance.OnPausePressed += GameInput_OnPausePressed;

        // Прив'язуємо функції до кнопок через код (AddListener)
        if (resumeButton != null) resumeButton.onClick.AddListener(Resume);
        if (saveButton != null) saveButton.onClick.AddListener(SaveGame);
        if (exitButton != null) exitButton.onClick.AddListener(ExitToMainMenu);
    }

    private void OnDestroy()
    {
        // Відписуємось від подій для запобігання витоку пам'яті
        if (GameInput.Instance != null)
        {
            GameInput.Instance.OnPausePressed -= GameInput_OnPausePressed;
        }

        // Рекомендується очищати слухачів кнопок при знищенні об'єкта
        if (resumeButton != null) resumeButton.onClick.RemoveListener(Resume);
        if (saveButton != null) saveButton.onClick.RemoveListener(SaveGame);
        if (exitButton != null) exitButton.onClick.RemoveListener(ExitToMainMenu);
    }

    private void GameInput_OnPausePressed(object sender, EventArgs e)
    {
        if (isPaused)
        {
            Resume();
        }
        else
        {
            Pause();
        }
    }

    public void Resume()
    {
        pauseMenuPanel.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    public void Pause()
    {
        if (Player.Instance != null && !Player.Instance.IsAlive()) return;

        pauseMenuPanel.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    // Методи тепер можна зробити private, оскільки ззовні (з Unity Event) їх більше ніхто не смикає
    private void SaveGame()
    {
        if (SaveLoadManager.Instance != null)
        {
            // 1. Беремо назву поточного світу точно так само, як ти це робиш у SaveLoadManager через PlayerPrefs
            string currentWorld = PlayerPrefs.GetString("SelectedWorld", "DefaultWorld");

            // 2. Викликаємо твій готовий метод збереження
            SaveLoadManager.Instance.SaveGame(currentWorld);

            Debug.Log($"[PauseMenu] Кнопка UI спрацювала. Світ '{currentWorld}' збережено через SaveLoadManager.");
        }
        else
        {
            Debug.LogError("Помилка: SaveLoadManager.Instance не знайдено на сцені!");
        }
    }

    private void ExitToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}