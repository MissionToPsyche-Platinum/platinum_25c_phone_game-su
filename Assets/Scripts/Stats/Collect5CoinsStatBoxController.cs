using UnityEngine;

public class Collect5CoinsStatBoxController : StatBoxController
{
    protected override void SetAppropriateVisual()
    {
        displayedText.text = "Collect more than 5 coins";
        
        if (StatsManager.instance.coinsCollected >= 5)
        {
            ShowCheckmark();
        }
        else
        {
            ShowX();
        }
    }
}