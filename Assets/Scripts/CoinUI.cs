using UnityEngine;
using UnityEngine.UI;

// Keeps the coin/crystal counter text in sync with the player's coin balance.
// Left-handed mirroring is handled by HorizontalMirrorUI on this element and its
// sibling icon, so this script is only responsible for the number text.
public class CoinUI : MonoBehaviour
{
    public Text coinText;

    void Update()
    {
        if (coinText != null && GameStateManager.Instance != null)
            coinText.text = GameStateManager.Instance.GetCoins().ToString();
    }
}
