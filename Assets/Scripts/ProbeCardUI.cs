using UnityEngine;
using UnityEngine.UI;

public class ProbeCardUI : MonoBehaviour
{
    [SerializeField] private Text nameText;
    [SerializeField] private Button upgradeButton;
    [SerializeField] private Text buttonLabel;
    [SerializeField] private Image cardBackground;
    [SerializeField] private GameObject coinIcon;

    [SerializeField] private GameObject[] levelIcons;

    [Header("Card Colors")]
    [SerializeField] private Color defaultColor = new Color(1.0f, 1.0f, 1.00f, 1f);
    [SerializeField] private Color maxedColor   = new Color(0.5f, 0.5f, 0.5f, 1f);

    private int _index;

    public void Setup(int index)
    {
        _index = index;
        nameText.text = ProbeUpgradeManager.Names[index];
        upgradeButton.onClick.AddListener(OnUpgradeClicked);
        Refresh();
    }

    public void Refresh()
    {
        var pm = ProbeUpgradeManager.Instance;
        int level = pm.GetLevel(_index);
        bool maxed = pm.IsMaxed(_index);

        for(int i = 0; i < 5; i++){
            levelIcons[i].SetActive(false);
        }

        for(int i = 0; i < level; i++){
            levelIcons[i].SetActive(true);
        }

        if(maxed){
            coinIcon.SetActive(false);
        } else {
            coinIcon.SetActive(true);
        }

        if (maxed)
        {
            buttonLabel.text = "MAX LEVEL";
            upgradeButton.interactable = false;
            if (cardBackground != null) cardBackground.color = maxedColor;
        }
        else
        {
            int cost = pm.GetUpgradeCost(_index);
            buttonLabel.text = "" + cost;
            upgradeButton.interactable = GameStateManager.Instance.GetCoins() >= cost;
            if (cardBackground != null) cardBackground.color = defaultColor;
        }
    }

    private void OnUpgradeClicked()
    {
        ProbeUpgradeManager.Instance.TryUpgrade(_index);
        GetComponentInParent<ProbeUpgradeScriptController>()?.RefreshAll();
    }
}
