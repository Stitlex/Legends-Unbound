using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveSlotUI : MonoBehaviour
{
    [Header("UI Елементи Слоту")]
    [SerializeField] private TextMeshProUGUI worldNameText;
    [SerializeField] private Button loadButton;
    [SerializeField] private Button deleteButton;

    private string worldName;

    public void Setup(string name, Action<string> onLoadCallback, Action<string> onDeleteCallback)
    {
        worldName = name;

        if (worldNameText != null)
            worldNameText.text = name;

        if (loadButton != null)
        {
            loadButton.onClick.RemoveAllListeners();
            loadButton.onClick.AddListener(() => onLoadCallback?.Invoke(worldName));
        }

        if (deleteButton != null)
        {
            deleteButton.onClick.RemoveAllListeners();
            deleteButton.onClick.AddListener(() => onDeleteCallback?.Invoke(worldName));
        }
    }
}