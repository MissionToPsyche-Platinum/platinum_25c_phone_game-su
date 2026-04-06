using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public class CheckpointChooser : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown stageDropdown;

    [SerializeField] private List<CheckpointEntry> selectableStages = new  List<CheckpointEntry>();

    void Start()
    {
        PopulateDropdown();
        RestoreLastSelection();

        stageDropdown.onValueChanged.AddListener(OnDropdownChanged);
    }

    private void PopulateDropdown()
    {
        stageDropdown.ClearOptions();

        List<string> options = new List<string>();
        foreach (CheckpointEntry stage in selectableStages)
            options.Add(stage.stage.ToString());

        stageDropdown.AddOptions(options);
    }

    private void RestoreLastSelection()
    {
        // Default to whatever stage GameStateManager already has stored
        GameStateManager.GameStage current = GameStateManager.Instance.startingGameStage;

        for (int i = 0; i < selectableStages.Count; i++)
        {
            if (selectableStages[i].stage == current)
            {
                stageDropdown.value = i;
                break;
            }
        }
    }

    private void OnDropdownChanged(int index)
    {
        GameStateManager.Instance.startingGameStage = selectableStages[index].stage;
        GameStateManager.Instance.startingScore = selectableStages[index].startingScore;
    }
}

[System.Serializable]
public class CheckpointEntry
{
    public GameStateManager.GameStage stage;
    public float startingScore;
}