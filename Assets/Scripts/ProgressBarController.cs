using System;
using UnityEngine;
using UnityEngine.UIElements;
using Slider = UnityEngine.UI.Slider;

public class ProgressBarController : MonoBehaviour
{
    [SerializeField] CheckpointSpawnScript checkpointSpawnScript;
    [SerializeField] Slider progressBar;
    [SerializeField] ScoreIncrement scoreIncrement;

    private void OnEnable()
    {
        // GameStateManager.Instance.OnStartPlaying += GameStateManager_OnStartPlaying;
        checkpointSpawnScript.OnCheckpointPassed += CheckpointSpawnScript_OnCheckpointPassed;
    }

    private void OnDisable()
    {
        // GameStateManager.Instance.OnStartPlaying -= GameStateManager_OnStartPlaying;
        checkpointSpawnScript.OnCheckpointPassed -= CheckpointSpawnScript_OnCheckpointPassed;
    }

    private void Update()
    {
        progressBar.value = scoreIncrement.GetCurrentScore();
    }

    private void CheckpointSpawnScript_OnCheckpointPassed(object sender, EventArgs e)
    {
        Debug.Log("ProgressBar heard OnCheckopintPassed");
        progressBar.value = 0;
        progressBar.minValue = checkpointSpawnScript.GetLastCheckpointScore();
        progressBar.maxValue = checkpointSpawnScript.GetNextCheckpointScore();
    }

    // private void GameStateManager_OnStartPlaying(object sender, GameStateManager.GameStateChangeEventArgs e)
    // {
    //     progressBar.value = 0;
    //     progressBar.maxValue = checkpointSpawnScript.GetNextCheckpointScore();
    // }
    
    
}