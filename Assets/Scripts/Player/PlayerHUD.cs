using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class PlayerHUD : MonoBehaviour
{
    [Header("Health UI Settings")]
    [SerializeField] private Image healthBarFill;
    [SerializeField] private TextMeshProUGUI healthText;

    [Header("Gold UI Settings")]
    [SerializeField] private TextMeshProUGUI goldText;

    [Header("Progression UI Settings")]
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private Image expBarFill;
    [SerializeField] private TextMeshProUGUI expText;

    [Header("Level Up")]
    [SerializeField] private TextMeshProUGUI levelUpText;
    [SerializeField] private float levelUpDisplayTime = 2.5f;

    private void Start()
    {
        if (Player.Instance != null)
        {
            Player.Instance.OnLevelUp += Player_OnLevelUp;
        }
    }

    private void OnDestroy()
    {
        if (Player.Instance != null)
        {
            Player.Instance.OnLevelUp -= Player_OnLevelUp;
        }
    }

    private void Update()
    {
        UpdateHealthUI();
        UpdateGoldUI();
        UpdateProgressionUI();
    }

    private void Player_OnLevelUp(object sender, System.EventArgs e)
    {
        if (levelUpText != null)
        {
            StopAllCoroutines();
            levelUpText.text = "LEVEL UP!";
            StartCoroutine(HideLevelUpText());
        }
    }

    private IEnumerator HideLevelUpText()
    {
        yield return new WaitForSeconds(levelUpDisplayTime);
        if (levelUpText != null)
            levelUpText.text = "";
    }

    private void UpdateHealthUI()
    {
        if (Player.Instance != null)
        {
            int currentHP = Player.Instance.GetCurrentHealth();
            int maxHP = Player.Instance.GetMaxHealth();

            if (healthText != null) healthText.text = $"HP {currentHP} / {maxHP}";
            if (healthBarFill != null && maxHP > 0)
            {
                healthBarFill.fillAmount = (float)currentHP / maxHP;
            }
        }
    }

    private void UpdateGoldUI()
    {
        int gold = InventoryManager.Instance.GetGold();
        if (InventoryManager.Instance != null)
        {
            if (goldText != null) goldText.text = $"COINS {gold}";
        }
    }

    private void UpdateProgressionUI()
    {
        if (Player.Instance != null)
        {
            int currentLvl = Player.Instance.GetLevel();
            int currentXp = Player.Instance.GetCurrentExp();
            int maxXp = Player.Instance.GetExpToNextLevel();

            if (levelText != null)
            {
                levelText.text = $"LVL {currentLvl}";
            }

            if (expText != null)
            {
                expText.text = $"XP {currentXp} / {maxXp}";
            }

            if (expBarFill != null && maxXp > 0)
            {
                expBarFill.fillAmount = (float)currentXp / maxXp;
            }
        }
    }
}