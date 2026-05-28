using UnityEngine;

public class InventoryController : MonoBehaviour
{
    public static InventoryController Instance { get; private set; }

    [SerializeField] private GameObject inventoryUIPanel;
    [SerializeField] private InventoryUI inventoryUI;

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
        inventoryUIPanel.SetActive(false);

        if (GameInput.Instance != null)
        {
            GameInput.Instance.OnInventoryToggled += ToggleInventory;
        }
    }

    private void OnDestroy()
    {
        if (GameInput.Instance != null)
        {
            GameInput.Instance.OnInventoryToggled -= ToggleInventory;
        }
    }

    public void ForceCloseInventory()
    {
        if (inventoryUIPanel != null)
        {
            inventoryUIPanel.SetActive(false);
            inventoryUI?.CloseDetailsPanel();
            Time.timeScale = 1f;
        }
    }

    private void ToggleInventory(object sender, System.EventArgs e)
    {
        if (CharacterMenuManager.Instance != null && CharacterMenuManager.Instance.IsOpen())
        {
            CharacterMenuManager.Instance.CloseCharacterMenu();
        }

        if (PauseMenuManager.Instance != null && PauseMenuManager.Instance.IsPaused())
        {
            PauseMenuManager.Instance.Resume();
        }

        if (inventoryUIPanel.activeSelf && inventoryUI.IsDetailsPanelOpen())
        {
            inventoryUI.CloseDetailsPanel();
            return;
        }

        bool isActive = !inventoryUIPanel.activeSelf;
        inventoryUIPanel.SetActive(isActive);

        if (isActive)
        {
            inventoryUI.UpdateInventoryList();
            Time.timeScale = 0f;
        }
        else
        {
            inventoryUI.CloseDetailsPanel();
            Time.timeScale = 1f;
        }
    }

    public bool IsInventoryOpen()
    {
        return inventoryUIPanel != null && inventoryUIPanel.activeSelf;
    }

    public void ForceOpenInventory()
    {
        Debug.Log("[InventoryController] ForceOpenInventory викликано");

        if (inventoryUIPanel != null && inventoryUI != null)
        {
            inventoryUIPanel.SetActive(true);
            inventoryUI.UpdateInventoryList();
            Time.timeScale = 0f;
            Debug.Log("[InventoryController] Інвентар успішно відкрито");
        }
        else
        {
            Debug.LogError("[InventoryController] inventoryUIPanel або inventoryUI не налаштовані!");
        }
    }
}