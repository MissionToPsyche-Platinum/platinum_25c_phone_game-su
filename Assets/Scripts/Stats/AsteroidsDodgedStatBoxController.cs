using UnityEngine;

public class AsteroidsDodgedStatBoxController : StatBoxController
{

    protected override void SetAppropriateVisual()
    {
        displayedText.text = "Asteroids Dodged: " + StatsManager.instance.asteroidsDodged.ToString();
        ShowNothingOnImage();
    }
}