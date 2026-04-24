using UnityEngine;
using UnityEngine.UI;

public class PowerUpCardUI : MonoBehaviour
{
    [SerializeField] private Text nameText;
    [SerializeField] private Text levelText;
    [SerializeField] private Text statsText;
    [SerializeField] private Button upgradeButton;
    [SerializeField] private Text buttonLabel;
    [SerializeField] private Image cardBackground;

    [Header("Card Colors")]
    [SerializeField] private Color defaultColor = new Color(0.75f, 0.75f, 1.00f, 1f);
    [SerializeField] private Color maxedColor   = new Color(1.00f, 0.84f, 0.00f, 1f);

    private int _index;

    public void Setup(int index)
    {
        _index = index;
        nameText.text = PowerUpUpgradeManager.Names[index];
        upgradeButton.onClick.AddListener(OnUpgradeClicked);
        Refresh();
    }

    public void Refresh()
    {
        var pm = PowerUpUpgradeManager.Instance;
        int level = pm.GetLevel(_index);
        bool maxed = pm.IsMaxed(_index);
        int spawnBonus = level * 10;
        int durationBonus = level * 10;

        levelText.text = maxed ? "Level MAX" : "Level " + level + " / 10";
        statsText.text = "+" + spawnBonus + "% spawn  |  +" + durationBonus + "% duration";

        if (maxed)
        {
            buttonLabel.text = "MAX LEVEL";
            upgradeButton.interactable = false;
            if (cardBackground != null) cardBackground.color = maxedColor;
        }
        else
        {
            int cost = pm.GetUpgradeCost(_index);
            buttonLabel.text = "Upgrade (" + cost + " coins)";
            upgradeButton.interactable = GameStateManager.Instance.GetCoins() >= cost;
            if (cardBackground != null) cardBackground.color = defaultColor;
        }
    }

    private void OnUpgradeClicked()
    {
        PowerUpUpgradeManager.Instance.TryUpgrade(_index);
        GetComponentInParent<PowerUpUpgradeScreenController>()?.RefreshAll();
    }
}
