using UnityEngine;
using UnityEngine.UI;

public class CoinUI : MonoBehaviour
{
    public Text coinText;

    void Update()
    {
        if (coinText != null && GameStateManager.Instance != null)
        {
            coinText.text = GameStateManager.Instance.GetCoins().ToString();
        }
    }
}