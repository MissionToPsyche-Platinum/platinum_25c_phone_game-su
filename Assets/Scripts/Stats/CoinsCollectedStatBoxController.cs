using UnityEngine;

public class CoinsCollectedStatBoxController : StatBoxController
{
    protected override void SetAppropriateVisual()
    {
        string value = StatsManager.instance.coinsCollected.ToString();
        if (valueText != null)
        {
            displayedText.text = "Coins Collected";
            valueText.text = value;
        }
        else
        {
            displayedText.text = "Coins Collected: " + value;
        }
        ShowNothingOnImage();
    }
}