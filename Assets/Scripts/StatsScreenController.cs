using UnityEngine;
using UnityEngine.UI;

public class StatsScreenController : MonoBehaviour
{

    [SerializeField] private Text highScoreText;
    [SerializeField] private Text totalDistanceText;
    [SerializeField] private Text asteroidsAvoidedText;
    [SerializeField] private Text coinsCollectedText;

    private int highScore = -1;
    private float totalDistance = 0; // in AU
    private int asteroidsAvoided = 0;
    private int coinsCollected = 0;

    //number of km in 0.1 light years
    private float distanceConversion = 946073047258.08f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        RefreshAll();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void updateHighScore(int score){
        if(score > highScore) highScore = score;
        RefreshAll();
    }

    public void updateTotalDistance(float distance){
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
            highScoreText.text = "-";
        }

        totalDistanceText.text = totalDistance + "AU";

        asteroidsAvoidedText.text = "" + StatsManager.instance.asteroidsDodged;

        coinsCollectedText.text = "" + coinsCollected;
    }
}
