using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenuManager : MonoBehaviour
{
    public static PauseMenuManager Instance { get; private set; }

    [Header("UI Elements")]
    [SerializeField] private GameObject pauseMenuPanel;

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

        GameInput.Instance.OnPausePressed += GameInput_OnPausePressed;

        if (resumeButton != null) resumeButton.onClick.AddListener(Resume);
        if (saveButton != null) saveButton.onClick.AddListener(SaveGame);
        if (exitButton != null) exitButton.onClick.AddListener(ExitToMainMenu);
    }

    private void OnDestroy()
    {
        if (GameInput.Instance != null)
        {
            GameInput.Instance.OnPausePressed -= GameInput_OnPausePressed;
        }

        if (resumeButton != null) resumeButton.onClick.RemoveListener(Resume);
        if (saveButton != null) saveButton.onClick.RemoveListener(SaveGame);
        if (exitButton != null) exitButton.onClick.RemoveListener(ExitToMainMenu);
    }

    public bool IsPaused() => isPaused;

    public void Resume()
    {
        pauseMenuPanel.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    public void Pause()
    {
        if (Player.Instance != null && !Player.Instance.IsAlive()) return;

        CloseAllOtherMenus();

        pauseMenuPanel.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
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

    private void CloseAllOtherMenus()
    {
        if (CharacterMenuManager.Instance != null && CharacterMenuManager.Instance.IsOpen())
        {
            CharacterMenuManager.Instance.CloseCharacterMenu();
        }

        if (InventoryController.Instance != null && InventoryController.Instance.IsInventoryOpen())
        {
            InventoryController.Instance.ForceCloseInventory();
        }
    }

    private void SaveGame()
    {
        if (SaveLoadManager.Instance != null)
        {
            string currentWorld = PlayerPrefs.GetString("SelectedWorld", "DefaultWorld");

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