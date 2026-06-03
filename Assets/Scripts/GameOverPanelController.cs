using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class GameOverPanelController : MonoBehaviour
{
    [SerializeField] private Button restartGameButton;
    [SerializeField] private Button shopButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject gameScreenPanel;
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private GameObject statsScreenPanel;
    [SerializeField] private GameObject startPanel;

    [SerializeField] private Text missionTitleText;
    [SerializeField] private Text missionReportText;
    [SerializeField] private Text funFactText;
    [SerializeField] private DistanceTracker distanceTracker;
    [SerializeField] private ScoreIncrement scoreIncrement;
    
    [Header("Fun Facts")]
    [SerializeField] private List<string> funFacts;

    private StatsScreenController statsScreenController;

    private void Awake()
    {
        if (restartGameButton != null)
            restartGameButton.onClick.AddListener(RestartGameButtonClicked);

        if (shopButton != null)
            shopButton.onClick.AddListener(ShopButtonClicked);
        
        if (exitButton != null)
            exitButton.onClick.AddListener(ExitButtonClicked);
        
        statsScreenController = statsScreenPanel.GetComponent<StatsScreenController>();
    }

    // Fires every time the panel is shown (ProbeHealth re-activates it on death).
    private void OnEnable()
    {
        PopulateReport();
    }

    // Falls back to lookups so the report still works even if the scene's
    // inspector references were not wired up.
    private void ResolveReferences()
    {
        if (missionReportText == null)
        {
            Transform t = transform.Find("ScoreValue");
            if (t != null)
                missionReportText = t.GetComponent<Text>();
        }

        if (missionTitleText == null)
        {
            Transform t = transform.Find("MissionTerminatedTitle");
            if (t != null)
                missionTitleText = t.GetComponent<Text>();
        }

        if (distanceTracker == null)
            distanceTracker = FindFirstObjectByType<DistanceTracker>();

        if (scoreIncrement == null)
            scoreIncrement = FindFirstObjectByType<ScoreIncrement>();
    }

    private void PopulateReport()
    {
        ResolveReferences();

        if (missionTitleText != null)
            missionTitleText.text = "MISSION TERMINATED";

        if (missionReportText == null)
        {
            Debug.LogWarning("GameOverPanelController: missionReportText is not assigned " +
                             "and no 'ScoreValue' child was found; Mission Report cannot be shown.");
            return;
        }

        double distanceKm = distanceTracker != null ? distanceTracker.DistanceKm : 0.0;
        statsScreenController.updateTotalDistance(distanceKm); 
        int finalScore = scoreIncrement != null ? scoreIncrement.FinalScore : 0;
        statsScreenController.updateHighScore(finalScore);
        GameStateManager.Instance.UpdateAvailableCheckpoints(finalScore);
        int coinsCollected = StatsManager.instance != null ? StatsManager.instance.coinsCollected : 0;
        int asteroidsDodged = StatsManager.instance != null ? StatsManager.instance.asteroidsDodged : 0;

        StringBuilder report = new StringBuilder();
        report.AppendLine($"Distance Traveled:");
        report.AppendLine($"{distanceKm:N0} km");
        report.AppendLine();
        report.AppendLine($"Final Score:");
        report.AppendLine($"{finalScore:N0}");
        report.AppendLine();
        report.AppendLine($"Coins Collected:");
        report.AppendLine($"{coinsCollected}");
        report.AppendLine();
        report.AppendLine($"Asteroids Dodged:");
        report.AppendLine($"{asteroidsDodged}");

        missionReportText.text = report.ToString();
        
        funFactText.text = funFacts[Random.Range(0, funFacts.Count)];
    }

    public void RestartGameButtonClicked()
    {
        MenuNavigationButtons.Instance.PlayButtonSound();
        // Reloads the scene to reset everything (consistent with your Pause Menu Restart)
        GameStateManager.Instance.EnterPlayingState();
        MenuNavigationButtons.Instance.SwitchPanels(gameOverPanel, gameScreenPanel);
    }

    public void ShopButtonClicked()
    {
        MenuNavigationButtons.Instance.PlayButtonSound();
        this.gameObject.SetActive(false);
        GameStateManager.Instance.EnterMenuState();
        MenuNavigationButtons.Instance.SwitchPanels(gameOverPanel, shopPanel);
    }
    
    public void ExitButtonClicked()
    {
        MenuNavigationButtons.Instance.PlayButtonSound();
        this.gameObject.SetActive(false);
        GameStateManager.Instance.EnterMenuState();
        MenuNavigationButtons.Instance.SwitchPanels(gameOverPanel, startPanel);
    }
}