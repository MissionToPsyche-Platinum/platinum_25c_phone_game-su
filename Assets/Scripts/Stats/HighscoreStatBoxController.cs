using UnityEngine;

public class HighscoreStatBoxController : StatBoxController
{
    protected override void SetAppropriateVisual()
    {
        displayedText.text = "Highscore: " + Mathf.FloorToInt(StatsManager.instance.highscore).ToString();
        ShowNothingOnImage();
    }
}