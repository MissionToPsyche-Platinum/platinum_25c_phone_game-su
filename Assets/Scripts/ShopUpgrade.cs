using UnityEngine;
using UnityEngine.UI;

public class ShopUpgrades : MonoBehaviour
{
    private const int MaxLevel = 5;

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
        return 10 + (GameStateManager.Instance.maxHealthLevel * 10);
    }

    int GetArmorCost()
    {
        return 10 + (GameStateManager.Instance.armorLevel * 10);
    }

    int GetSpeedCost()
    {
        return 10 + (GameStateManager.Instance.speedLevel * 10);
    }

    void BuyHealthUpgrade()
    {
        if (GameStateManager.Instance.maxHealthLevel >= MaxLevel) return;
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
        if (GameStateManager.Instance.armorLevel >= MaxLevel) return;
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
        if (GameStateManager.Instance.speedLevel >= MaxLevel) return;
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
            bool healthMaxed = GameStateManager.Instance.maxHealthLevel >= MaxLevel;
            healthUpgradeText.text = "Max Health +" + GameStateManager.Instance.maxHealthLevel +
                                     (healthMaxed ? "\nMAXED" : "\nCost: " + GetHealthCost() + " coins");
        }
        if (healthUpgradeButton != null)
        {
            bool healthMaxed = GameStateManager.Instance.maxHealthLevel >= MaxLevel;
            healthUpgradeButton.interactable = !healthMaxed && (currentCoins >= GetHealthCost());
        }


        if (armorUpgradeText != null)
        {
            bool armorMaxed = GameStateManager.Instance.armorLevel >= MaxLevel;
            int armorChance = GameStateManager.Instance.armorLevel * 10;
            armorUpgradeText.text = "Armor (" + armorChance + "% dodge)" +
                                    (armorMaxed ? "\nMAXED" : "\nCost: " + GetArmorCost() + " coins");
        }
        if (armorUpgradeButton != null)
        {
            bool armorMaxed = GameStateManager.Instance.armorLevel >= MaxLevel;
            armorUpgradeButton.interactable = !armorMaxed && (currentCoins >= GetArmorCost());
        }


        if (speedUpgradeText != null)
        {
            bool speedMaxed = GameStateManager.Instance.speedLevel >= MaxLevel;
            speedUpgradeText.text = "Speed +" + GameStateManager.Instance.speedLevel +
                                   (speedMaxed ? "\nMAXED" : "\nCost: " + GetSpeedCost() + " coins");
        }
        if (speedUpgradeButton != null)
        {
            bool speedMaxed = GameStateManager.Instance.speedLevel >= MaxLevel;
            speedUpgradeButton.interactable = !speedMaxed && (currentCoins >= GetSpeedCost());
        }
    }
}