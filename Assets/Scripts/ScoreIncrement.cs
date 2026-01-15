using UnityEngine;
using UnityEngine.UI;

public class ScoreIncrement : MonoBehaviour
{
    public Text scoreText;
    public float scorePerSecond = 10f; // Modify to change score per second
    
    private float currentScore;
    
    [SerializeField] private int finalScore; // Can be grabbed by other sections

    public int FinalScore
    {
        get { return finalScore; }
    }

    void Update()
    {
        currentScore += scorePerSecond * Time.deltaTime;
        finalScore = Mathf.FloorToInt(currentScore);
        scoreText.text = "Score: " + finalScore.ToString();
    }
}