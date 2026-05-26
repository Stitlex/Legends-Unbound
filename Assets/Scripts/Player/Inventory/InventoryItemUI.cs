using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class InventoryItemUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private Button itemButton;
    [SerializeField] private Image buttonBackground;

    [Header("Color Settings")]
    [SerializeField] private Color equippedColor = new Color(0.2f, 0.6f, 0.2f, 1f);
    [SerializeField] private Color normalColor = new Color(0.25f, 0.25f, 0.25f, 1f);

    [Header("Text Color Settings")]
    [SerializeField] private Color textEquippedColor = Color.green;
    [SerializeField] private Color textNormalColor = Color.white;

    private string itemId;
    private string itemType;
    private Action<string, string> onSelected;

    public void Setup(string id, string name, string type, int count, Action<string, string> onSelectCallback)
    {
        itemId = id;
        itemType = type;
        itemNameText.text = count > 1 ? $"{name} (x{count})" : name;
        onSelected = onSelectCallback;

        itemButton.onClick.RemoveAllListeners();
        itemButton.onClick.AddListener(() => onSelected?.Invoke(itemId, itemType));
    }

    public void SetEquippedVisual(bool isEquipped)
    {
        if (isEquipped)
        {
            itemNameText.color = textEquippedColor;

            if (buttonBackground != null)
            {
                buttonBackground.color = equippedColor;
            }

            if (!itemNameText.text.Contains("[E]"))
            {
                itemNameText.text += " [E]";
            }
        }
        else
        {
            itemNameText.color = textNormalColor;

            if (buttonBackground != null)
            {
                buttonBackground.color = normalColor;
            }

            itemNameText.text = itemNameText.text.Replace(" [E]", "");
        }
    }
}