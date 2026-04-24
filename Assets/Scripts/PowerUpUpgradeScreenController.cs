using UnityEngine;
using UnityEngine.UI;

public class PowerUpUpgradeScreenController : MonoBehaviour
{
    [Header("Cards - assign indices 0 through 4 in order")]
    [SerializeField] private PowerUpCardUI[] cards;

    [Header("Info Labels")]
    [SerializeField] private Text coinDisplay;

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

        if (coinDisplay != null)
            coinDisplay.text = "Coins: " + GameStateManager.Instance.GetCoins();
    }
}
