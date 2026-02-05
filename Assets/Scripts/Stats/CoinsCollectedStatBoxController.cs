using UnityEngine;

public class CoinsCollectedStatBoxController : StatBoxController
{
    protected override void SetAppropriateVisual()
    {
        displayedText.text = "Coins Collected: " + StatsManager.instance.coinsCollected.ToString();
        ShowNothingOnImage();
    }
}