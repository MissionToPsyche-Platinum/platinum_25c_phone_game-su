using System;
using UnityEngine;
using UnityEngine.UI;

public class ScoreIncrement : MonoBehaviour
{
    public Text scoreText;
    public float scorePerSecond = 10f; // Modify to change score per second
    
    private float currentScore;
    [SerializeField] private float scoreMultiplier = 1f;

    private bool scoreUpdate = true;
    
    [SerializeField] private int finalScore; // Can be grabbed by other sections

    CheckpointSpawnScript checkpointSpawnScript;

    private void Start()
    {
        GameStateManager.Instance.OnStartPlaying += OnStartPlaying;

        GameObject checkpointSpawn = GameObject.Find("CheckpointSpawner");
        checkpointSpawnScript = checkpointSpawn.GetComponent<CheckpointSpawnScript>();
        checkpointSpawnScript.OnCheckpointReached += OnCheckpointReached;
        checkpointSpawnScript.OnCheckpointPassed += OnCheckpointPassed;
    }

    private void OnDestroy()
    {
        GameStateManager.Instance.OnStartPlaying -= OnStartPlaying;
        checkpointSpawnScript.OnCheckpointReached -= OnCheckpointReached;
        checkpointSpawnScript.OnCheckpointPassed -= OnCheckpointPassed;
    }

    public int FinalScore
    {
        get { return finalScore; }
    }

    void Update()
    {
        if(scoreUpdate){
            currentScore += scorePerSecond * Time.deltaTime * scoreMultiplier;
            finalScore = Mathf.FloorToInt(currentScore);
            scoreText.text = finalScore.ToString();
        }
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
        currentScore = GameStateManager.Instance.startingScore;
        SetScoreMultiplier(1f);
    }

    public float GetCurrentScore()
    {
        return currentScore;
    }
    
    private void OnCheckpointReached(object sender, EventArgs e)
    {
        scoreUpdate = false;
    }

    private void OnCheckpointPassed(object sender, EventArgs e)
    {
        scoreUpdate = true;
    }
}