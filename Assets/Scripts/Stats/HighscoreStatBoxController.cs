using UnityEngine;

public class HighscoreStatBoxController : StatBoxController
{
    protected override void SetAppropriateVisual()
    {
        string value = Mathf.FloorToInt(StatsManager.instance.highscore).ToString();
        if (valueText != null)
        {
            displayedText.text = "Highscore";
            valueText.text = value;
        }
        else
        {
            displayedText.text = "Highscore: " + value;
        }
        ShowNothingOnImage();
    }
}