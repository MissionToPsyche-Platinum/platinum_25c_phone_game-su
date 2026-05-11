using UnityEngine;
using UnityEngine.UI;

public class ComponentsScreenController : MonoBehaviour
{
    [Header("Cards - assign indices 0 through 7 in order")]
    [SerializeField] private ComponentCardUI[] cards;

    [Header("Info Labels")]
    [SerializeField] private Text coinDisplay;
    [SerializeField] private Text equippedLeftText;
    [SerializeField] private Text equippedRightText;

    private void Start()
    {
        for (int i = 0; i < cards.Length; i++)
            cards[i].Setup(i);
    }

    private void OnEnable()
    {
        RefreshAll();
    }

    public void RefreshAll()
    {
        foreach (var card in cards)
            card.Refresh();

        coinDisplay.text = "" + GameStateManager.Instance.GetCoins();

        int left  = ComponentManager.Instance.GetEquippedLeft();
        int right = ComponentManager.Instance.GetEquippedRight();

        // equippedLeftText.text  = left  == -1 ? "Left Slot: None"  : "Left: "  + ComponentManager.Names[left];
        // equippedRightText.text = right == -1 ? "Right Slot: None" : "Right: " + ComponentManager.Names[right];
    }
}