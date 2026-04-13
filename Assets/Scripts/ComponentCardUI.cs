using UnityEngine;
using UnityEngine.UI;

public class ComponentCardUI : MonoBehaviour
{
    [SerializeField] private Text   nameText;
    [SerializeField] private Text   descText;
    [SerializeField] private Button actionButton;
    [SerializeField] private Text   buttonLabel;
    [SerializeField] private Image  cardBackground;

    [Header("Card Colors")]
    [SerializeField] private Color lockedColor   = new Color(0.40f, 0.40f, 0.40f, 1f);
    [SerializeField] private Color unlockedColor = new Color(0.75f, 0.75f, 1.00f, 1f);
    [SerializeField] private Color equippedColor = new Color(0.30f, 0.90f, 0.40f, 1f);

    private int _index;

    public void Setup(int index)
    {
        _index = index;
        nameText.text = ComponentManager.Names[index];
        descText.text = ComponentManager.Descriptions[index];
        actionButton.onClick.AddListener(OnActionClicked);
        Refresh();
    }

    public void Refresh()
    {
        var  cm        = ComponentManager.Instance;
        bool unlocked  = cm.IsUnlocked(_index);
        bool equipped  = cm.IsEquipped(_index);
        int  cost      = ComponentManager.Costs[_index];
        bool canAfford = GameStateManager.Instance.GetCoins() >= cost;

        if (!unlocked)
        {
            buttonLabel.text          = "Unlock " + cost + " coins";
            actionButton.interactable = canAfford;
            if (cardBackground != null) cardBackground.color = lockedColor;
        }
        else if (equipped)
        {
            buttonLabel.text          = "Equipped";
            actionButton.interactable = true;
            if (cardBackground != null) cardBackground.color = equippedColor;
        }
        else
        {
            buttonLabel.text          = "Equip";
            actionButton.interactable = true;
            if (cardBackground != null) cardBackground.color = unlockedColor;
        }
    }

    private void OnActionClicked()
    {
        var cm = ComponentManager.Instance;
        if (!cm.IsUnlocked(_index))
            cm.TryUnlock(_index);
        else if (_index < 4)
            cm.EquipLeft(_index);
        else
            cm.EquipRight(_index);

        GetComponentInParent<ComponentsScreenController>()?.RefreshAll();
    }
}
