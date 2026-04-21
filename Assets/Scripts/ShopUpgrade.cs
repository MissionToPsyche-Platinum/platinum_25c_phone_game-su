using UnityEngine;
using UnityEngine.UI;

public class ShopUpgrades : MonoBehaviour
{
    [Header("Health Upgrade")]
    public Text healthUpgradeText;
    public Button healthUpgradeButton;

    [Header("Armor Upgrade")]
    public Text armorUpgradeText;
    public Button armorUpgradeButton;

    [Header("Speed Upgrade")]
    public Text speedUpgradeText;
    public Button speedUpgradeButton;

    void Start()
    {
        
        if (healthUpgradeButton != null && healthUpgradeText == null)
        {
            healthUpgradeText = healthUpgradeButton.GetComponentInChildren<Text>();
        }
        if (armorUpgradeButton != null && armorUpgradeText == null)
        {
            armorUpgradeText = armorUpgradeButton.GetComponentInChildren<Text>();
        }
        if (speedUpgradeButton != null && speedUpgradeText == null)
        {
            speedUpgradeText = speedUpgradeButton.GetComponentInChildren<Text>();
        }

        UpdateUI();

        
        if (healthUpgradeButton != null)
            healthUpgradeButton.onClick.AddListener(BuyHealthUpgrade);
        if (armorUpgradeButton != null)
            armorUpgradeButton.onClick.AddListener(BuyArmorUpgrade);
        if (speedUpgradeButton != null)
            speedUpgradeButton.onClick.AddListener(BuySpeedUpgrade);
    }

    void Update()
    {
        UpdateUI();
    }

    int GetHealthCost()
    {
        return 3 + GameStateManager.Instance.maxHealthLevel;
    }

    int GetArmorCost()
    {
        return 10 + (GameStateManager.Instance.armorLevel * 10);
    }

    int GetSpeedCost()
    {
        return 3 + GameStateManager.Instance.speedLevel;
    }

    void BuyHealthUpgrade()
    {
        int cost = GetHealthCost();
        int currentCoins = GameStateManager.Instance.GetCoins();

        if (currentCoins >= cost)
        {
            GameStateManager.Instance.AddCoins(-cost);
            GameStateManager.Instance.maxHealthLevel += 1;
            GameStateManager.Instance.startingHealth += 1;

            Debug.Log("Health upgraded! Level: " + GameStateManager.Instance.maxHealthLevel);
            UpdateUI();
        }
        else
        {
            Debug.Log("Not enough coins!");
        }
    }

    void BuyArmorUpgrade()
    {
        int cost = GetArmorCost();
        int currentCoins = GameStateManager.Instance.GetCoins();

        if (currentCoins >= cost)
        {
            GameStateManager.Instance.AddCoins(-cost);
            GameStateManager.Instance.armorLevel += 1;

            Debug.Log("Armor upgraded! Level: " + GameStateManager.Instance.armorLevel);
            UpdateUI();
        }
        else
        {
            Debug.Log("Not enough coins!");
        }
    }

    void BuySpeedUpgrade()
    {
        int cost = GetSpeedCost();
        int currentCoins = GameStateManager.Instance.GetCoins();

        if (currentCoins >= cost)
        {
            GameStateManager.Instance.AddCoins(-cost);
            GameStateManager.Instance.speedLevel += 1;

            Debug.Log("Speed upgraded! Level: " + GameStateManager.Instance.speedLevel);
            UpdateUI();
        }
        else
        {
            Debug.Log("Not enough coins!");
        }
    }

    void UpdateUI()
    {
        int currentCoins = GameStateManager.Instance.GetCoins();

        
        if (healthUpgradeText != null)
        {
            int cost = GetHealthCost();
            healthUpgradeText.text = "Max Health +" + GameStateManager.Instance.maxHealthLevel +
                                     "\nCost: " + cost + " coins";
        }
        if (healthUpgradeButton != null)
        {
            healthUpgradeButton.interactable = (currentCoins >= GetHealthCost());
        }

        
        if (armorUpgradeText != null)
        {
            int cost = GetArmorCost();
            int armorChance = GameStateManager.Instance.armorLevel * 10;
            armorUpgradeText.text = "Armor (" + armorChance + "% dodge)\nCost: " + cost + " coins";
        }
        if (armorUpgradeButton != null)
        {
            armorUpgradeButton.interactable = (currentCoins >= GetArmorCost());
        }

        
        if (speedUpgradeText != null)
        {
            int cost = GetSpeedCost();
            speedUpgradeText.text = "Speed +" + GameStateManager.Instance.speedLevel +
                                   "\nCost: " + cost + " coins";
        }
        if (speedUpgradeButton != null)
        {
            speedUpgradeButton.interactable = (currentCoins >= GetSpeedCost());
        }
    }
}