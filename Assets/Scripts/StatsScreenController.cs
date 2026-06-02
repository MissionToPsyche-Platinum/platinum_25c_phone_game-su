using UnityEngine;
using UnityEngine.UI;

public class StatsScreenController : MonoBehaviour
{

    [Header("Stat fields to update")]
    [SerializeField] private Text highScoreText;
    [SerializeField] private Text totalDistanceText;
    [SerializeField] private Text asteroidsAvoidedText;
    [SerializeField] private Text coinsCollectedText;
    [SerializeField] private Image reachPsycheImage;
    [SerializeField] private Text realDistanceTravelledText;
    [SerializeField] private Image passedArtemisImage;
    [SerializeField] private Image passedVoyagerImage;
    [SerializeField] private Text asteroidsHitText;

    [Header("Stuff for managing stats")]
    [SerializeField] private int PsycheScoreThreshold;
    [SerializeField] private int ArtemisKmThreshold;
    [SerializeField] private double VoyagerKmThreshold;
    [SerializeField] private Sprite emptyStar;
    [SerializeField] private Sprite filledStar;

    private int highScore = -1;
    private double totalDistance = 0; // in km
    private int asteroidsAvoided = 0;
    private int coinsCollected = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        RefreshAll();
    }

    public void updateHighScore(int score){
        if(score > highScore) highScore = score;
        RefreshAll();
    }

    public void updateTotalDistance(double distance){
        totalDistance += distance;
        RefreshAll();
    }

    public void updateAsteroidsAvoided(int numAsteroids){
        asteroidsAvoided += numAsteroids;
        RefreshAll();
    }

    public void updateCoinsCollected(int coins){
        coinsCollected += coins;
        RefreshAll();
    }

    public void RefreshAll()
    {
        if(highScore > 0){
            highScoreText.text = "" + highScore;
        } else {
            highScoreText.text = "0";
        }

        double kmToAU = 149597870.7;
        double kmToLY = 9460730472580.8;

        if(totalDistance < kmToAU){
            totalDistanceText.text = (long)totalDistance + " km";
        } else if(totalDistance < kmToLY){
            totalDistanceText.text = (long)(totalDistance / kmToAU) + " AU";
        } else {
            totalDistanceText.text = (long)(totalDistance / kmToLY) + " LY";
        }
        

        asteroidsAvoidedText.text = "" + StatsManager.instance.asteroidsDodged;

        coinsCollectedText.text = "" + coinsCollected;
        
        asteroidsHitText.text = "" + GameStateManager.Instance.TotalAsteroidsHit;

        SetStar(reachPsycheImage, highScore >= PsycheScoreThreshold);
        SetStar(passedArtemisImage, GameStateManager.Instance.longestRealDistance >= ArtemisKmThreshold);
        SetStar(passedVoyagerImage, highScore >= VoyagerKmThreshold);

        if(GameStateManager.Instance.longestRealDistance < kmToAU){
            realDistanceTravelledText.text = (long)GameStateManager.Instance.longestRealDistance + " km";
        } else if(totalDistance < kmToLY){
            realDistanceTravelledText.text = (long)(GameStateManager.Instance.longestRealDistance / kmToAU) + " AU";
        } else {
            realDistanceTravelledText.text = (long)(GameStateManager.Instance.longestRealDistance / kmToLY) + " LY";
        }
    }

    private void SetStar(Image image, bool isCompleted)
    {
        if (isCompleted)
        {
            image.sprite = filledStar;
            image.color = Color.white;
        }
        else
        {
            image.sprite = emptyStar;
            image.color = new Color(0, 0, 0, .7f);
        }
    }
}