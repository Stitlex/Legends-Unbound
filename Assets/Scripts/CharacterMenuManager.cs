using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterMenuManager : MonoBehaviour
{
    public static CharacterMenuManager Instance { get; private set; }

    [Header("UI Panel")]
    [SerializeField] private GameObject characterMenuPanel;

    [Header("Player Stats Texts")]
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI strengthText;
    [SerializeField] private TextMeshProUGUI agilityText;
    [SerializeField] private TextMeshProUGUI expText;
    [SerializeField] private TextMeshProUGUI goldText;

    [Header("Skill Points")]
    [SerializeField] private TextMeshProUGUI skillPointsText;

    [Header("Buttons")]
    [SerializeField] private Button closeButton;
    [SerializeField] private Button openInventoryButton;

    [Header("Upgrade Buttons")]
    [SerializeField] private Button healthUpgradeButton;
    [SerializeField] private Button strengthUpgradeButton;
    [SerializeField] private Button agilityUpgradeButton;

    private bool isOpen = false;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        characterMenuPanel.SetActive(false);

        if (GameInput.Instance != null)
        {
            GameInput.Instance.OnCharacterMenuToggled += ToggleCharacterMenu;
        }

        if (healthUpgradeButton != null)
            healthUpgradeButton.onClick.AddListener(() => UpgradeStat("health"));

        if (strengthUpgradeButton != null)
            strengthUpgradeButton.onClick.AddListener(() => UpgradeStat("strength"));

        if (agilityUpgradeButton != null)
            agilityUpgradeButton.onClick.AddListener(() => UpgradeStat("agility"));
    }

    private void OnDestroy()
    {
        if (GameInput.Instance != null)
        {
            GameInput.Instance.OnCharacterMenuToggled -= ToggleCharacterMenu;
        }
    }

    public void CloseCharacterMenu()
    {
        if (characterMenuPanel != null)
            characterMenuPanel.SetActive(false);

        isOpen = false;
        Time.timeScale = 1f;
    }

    public bool IsOpen() => isOpen;

    private void ToggleCharacterMenu(object sender, System.EventArgs e)
    {
        if (InventoryController.Instance != null && InventoryController.Instance.IsInventoryOpen())
            InventoryController.Instance.ForceCloseInventory();

        if (PauseMenuManager.Instance != null && PauseMenuManager.Instance.IsPaused())
            PauseMenuManager.Instance.Resume();

        bool newState = !isOpen;
        characterMenuPanel.SetActive(newState);
        isOpen = newState;

        if (newState)
        {
            UpdateAllStats();
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1f;
        }
    }

    public void UpdateAllStats()
    {
        if (Player.Instance == null) return;

        if (levelText != null) levelText.text = $"Рівень: {Player.Instance.GetLevel()}";
        if (healthText != null) healthText.text = $"HP: {Player.Instance.GetCurrentHealth()} / {Player.Instance.GetMaxHealth()}";
        if (strengthText != null) strengthText.text = $"Сила: {Player.Instance.GetStrength()}";
        if (agilityText != null) agilityText.text = $"Спритність: {Player.Instance.GetAgility()}";
        if (expText != null) expText.text = $"Досвід: {Player.Instance.GetCurrentExp()} / {Player.Instance.GetExpToNextLevel()}";
        if (goldText != null) goldText.text = $"Золото: {InventoryManager.Instance.GetGold()}";
        if (skillPointsText != null) skillPointsText.text = $"Очки прокачки: {Player.Instance.GetSkillPoints()}";
    }

    private void UpgradeStat(string stat)
    {
        if (Player.Instance == null) return;

        bool success = Player.Instance.SpendSkillPoint(stat);

        if (success)
        {
            UpdateAllStats();
        }
    }

    private void CloseAllOtherMenus()
    {
        if (InventoryController.Instance != null && InventoryController.Instance.IsInventoryOpen())
        {
            InventoryController.Instance.ForceCloseInventory();
        }
        
        if (PauseMenuManager.Instance != null && PauseMenuManager.Instance.IsPaused())
        {
            PauseMenuManager.Instance.Resume();
        }
    }
}