using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CheckpointChooser : MonoBehaviour
{
    [SerializeField] private List<CheckpointEntry> selectableStages = new List<CheckpointEntry>();

    [SerializeField] private Button leftButton;
    [SerializeField] private Button rightButton;
    [SerializeField] private Text displayName;
    [SerializeField] private Image lockSprite;
    [SerializeField] private Button buyButton;
    [SerializeField] private TextMeshProUGUI buyButtonText;
    [SerializeField] private Button startGameButton;

    [SerializeField] GameObject checkpointSpawner;

    private CheckpointSpawnScript checkpointSpawnScript;
    private int index = 0;

    void Start()
    {
        leftButton.onClick.AddListener(OnLeftButtonPressed);
        rightButton.onClick.AddListener(OnRightButtonPressed);
        buyButton.onClick.AddListener(OnBuyButtonPressed);

        checkpointSpawnScript = checkpointSpawner.GetComponent<CheckpointSpawnScript>();
        UpdateDisplay();
    }

    void OnEnable()
    {
        if (checkpointSpawnScript != null)
            UpdateDisplay();
    }

    void OnDestroy()
    {
        leftButton.onClick.RemoveListener(OnLeftButtonPressed);
        rightButton.onClick.RemoveListener(OnRightButtonPressed);
        buyButton.onClick.RemoveListener(OnBuyButtonPressed);
    }

    private void OnLeftButtonPressed()
    {
        index = index > 0 ? (index - 1) % selectableStages.Count : selectableStages.Count - 1;
        GameStateManager.Instance.startingGameStage = selectableStages[index].stage;
        GameStateManager.Instance.startingScore = selectableStages[index].startingScore;
        checkpointSpawnScript.DecrementStartingCheckpoint();
        UpdateDisplay();
    }

    private void OnRightButtonPressed()
    {
        index = (index + 1) % selectableStages.Count;
        GameStateManager.Instance.startingGameStage = selectableStages[index].stage;
        GameStateManager.Instance.startingScore = selectableStages[index].startingScore;
        checkpointSpawnScript.IncrementStartingCheckpoint();
        UpdateDisplay();
    }

    private void OnBuyButtonPressed()
    {
        if (GameStateManager.Instance.TryUnlockStage(selectableStages[index].stage))
            UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        CheckpointEntry entry = selectableStages[index];
        bool available = GameStateManager.Instance.IsStageAvailable(entry.stage);
        bool locked = !GameStateManager.Instance.IsStageUnlocked(entry.stage);
        bool affordable = entry.unlockCost > 0;

        displayName.text = entry.displayName;
        lockSprite.gameObject.SetActive(!available);
        buyButton.gameObject.SetActive(available && locked);

        if (locked)
        {
            buyButtonText.text = "" + entry.unlockCost;
            buyButton.interactable = GameStateManager.Instance.GetCoins() >= entry.unlockCost;
        }

        if (startGameButton != null)
            startGameButton.interactable = !locked;
    }
}

[System.Serializable]
public class CheckpointEntry
{
    public GameStateManager.GameStage stage;
    public float startingScore;
    public string displayName;
    public int unlockCost;
}