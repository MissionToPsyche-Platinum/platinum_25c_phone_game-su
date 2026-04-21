using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CheckpointChooser : MonoBehaviour
{

    [SerializeField] private List<CheckpointEntry> selectableStages = new  List<CheckpointEntry>();

    [SerializeField] private Button leftButton;
    [SerializeField] private Button rightButton;

    [SerializeField] GameObject checkpointSpawner;

    private CheckpointSpawnScript checkpointSpawnScript;

    private int index = 0;

    void Start()
    {
        leftButton.onClick.AddListener(OnLeftButtonPressed);
        rightButton.onClick.AddListener(OnRightButtonPressed);

        checkpointSpawnScript = checkpointSpawner.GetComponent<CheckpointSpawnScript>();
    }

    void OnDestroy()
    {
        leftButton.onClick.RemoveListener(OnLeftButtonPressed);
        rightButton.onClick.RemoveListener(OnRightButtonPressed);
    }

    private void OnLeftButtonPressed(){
        index = index > 0 ? (index - 1) % selectableStages.Count : selectableStages.Count - 1;

        GameStateManager.Instance.startingGameStage = selectableStages[index].stage;
        GameStateManager.Instance.startingScore = selectableStages[index].startingScore;
        checkpointSpawnScript.DecrementStartingCheckpoint();
    }

    private void OnRightButtonPressed(){
        index = (index + 1) % selectableStages.Count;

        GameStateManager.Instance.startingGameStage = selectableStages[index].stage;
        GameStateManager.Instance.startingScore = selectableStages[index].startingScore;
        checkpointSpawnScript.IncrementStartingCheckpoint();
    }
}

[System.Serializable]
public class CheckpointEntry
{
    public GameStateManager.GameStage stage;
    public float startingScore;
}