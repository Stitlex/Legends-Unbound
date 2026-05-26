using UnityEngine;

public class InventoryController : MonoBehaviour
{
    [SerializeField] private GameObject inventoryUIPanel;
    [SerializeField] private InventoryUI inventoryUI;

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

    private void ToggleInventory(object sender, System.EventArgs e)
    {
        if (inventoryUI.IsDetailsPanelOpen())
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
}