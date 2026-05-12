using UnityEngine;

public class AsteroidsDodgedStatBoxController : StatBoxController
{

    protected override void SetAppropriateVisual()
    {
        string value = StatsManager.instance.asteroidsDodged.ToString();
        if (valueText != null)
        {
            displayedText.text = "Asteroids Dodged";
            valueText.text = value;
        }
        else
        {
            displayedText.text = "Asteroids Dodged: " + value;
        }
        ShowNothingOnImage();
    }
}