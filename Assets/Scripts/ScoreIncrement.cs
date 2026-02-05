using System;
using UnityEngine;
using UnityEngine.UI;

public class ScoreIncrement : MonoBehaviour
{
    public Text scoreText;
    public float scorePerSecond = 10f; // Modify to change score per second
    
    private float currentScore;
    [SerializeField] private float scoreMultiplier = 1f;
    
    [SerializeField] private int finalScore; // Can be grabbed by other sections

    private void Start()
    {
        GameStateManager.Instance.OnStartPlaying += OnStartPlaying;
    }

    private void OnDestroy()
    {
        GameStateManager.Instance.OnStartPlaying -= OnStartPlaying;
    }

    public int FinalScore
    {
        get { return finalScore; }
    }

    void Update()
    {
        currentScore += scorePerSecond * Time.deltaTime * scoreMultiplier;
        finalScore = Mathf.FloorToInt(currentScore);
        scoreText.text = "Score: " + finalScore.ToString();
    }

    public void ScaleScoreRate(float scalar){
        scorePerSecond *= scalar;
    }

    public void SetScoreMultiplier(float scoreScalar)
    {
        scoreMultiplier = scoreScalar;
    }
    
    private void OnStartPlaying(object sender, EventArgs e)
    {
        currentScore = 0;
        SetScoreMultiplier(1f);
    }

    public float GetCurrentScore()
    {
        return currentScore;
    }
}