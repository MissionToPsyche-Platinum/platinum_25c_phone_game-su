using System;
using UnityEngine;
using UnityEngine.UIElements;
using Slider = UnityEngine.UI.Slider;

public class ProgressBarController : MonoBehaviour
{
    [SerializeField] CheckpointSpawnScript checkpointSpawnScript;
    [SerializeField] Slider progressBar;
    [SerializeField] ScoreIncrement scoreIncrement;
    
    [Header("Crazy mode")]
    [SerializeField] float crazySpeed = 4000f;
    [SerializeField] int noMoreCheckpointsValue = 99999999;

    private bool crazyModeActive;
    private float crazyTimer;

    private void OnEnable()
    {
        // GameStateManager.Instance.OnStartPlaying += GameStateManager_OnStartPlaying;
        checkpointSpawnScript.OnCheckpointPassed += CheckpointSpawnScript_OnCheckpointPassed;
        GameStateManager.Instance.OnStartPlaying += GameStateManager_OnStartPlaying;
    }


    private void OnDisable()
    {
        // GameStateManager.Instance.OnStartPlaying -= GameStateManager_OnStartPlaying;
        checkpointSpawnScript.OnCheckpointPassed -= CheckpointSpawnScript_OnCheckpointPassed;
        GameStateManager.Instance.OnStartPlaying -= GameStateManager_OnStartPlaying;
    }

    private void Update()
    {
        if (crazyModeActive)
        {
            crazyTimer += Time.deltaTime;
            progressBar.value = Mathf.PingPong(crazyTimer * crazySpeed, progressBar.maxValue);
        }
        else
        {
            progressBar.value = scoreIncrement.GetCurrentScore();
        }
    }

    private void CheckpointSpawnScript_OnCheckpointPassed(object sender, EventArgs e)
    {
        // Debug.Log("ProgressBar heard OnCheckopintPassed");
        
        int nextCheckpointScore = checkpointSpawnScript.GetNextCheckpointScore();
        
        if (nextCheckpointScore >= noMoreCheckpointsValue)
        {
            crazyModeActive = true;
            crazyTimer = 0f;

            progressBar.minValue = 0f;
            progressBar.maxValue = 1f;
            progressBar.value = 0f;
        }
        else
        {
            crazyModeActive = false;

            progressBar.minValue = checkpointSpawnScript.GetLastCheckpointScore();
            progressBar.maxValue = nextCheckpointScore;
            progressBar.value = scoreIncrement.GetCurrentScore();
        }
    }

    private void GameStateManager_OnStartPlaying(object sender, GameStateManager.GameStateChangeEventArgs e)
    {
        crazyModeActive = false;
    }
}