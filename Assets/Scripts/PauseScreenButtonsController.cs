using UnityEngine;
using UnityEngine.UI;
public class PauseScreenButtonsController : MonoBehaviour
{
    [Header("Panels Can Switch To")]
    [SerializeField] private GameObject startScreenPanel;
    [SerializeField] private GameObject gameScreenPanel;
    [SerializeField] private GameObject pauseScreenPanel;
    [SerializeField] private GameObject settingsScreenPanel;

    [Header("Buttons")]
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button settingsButton;

    private void Awake()
    {
        resumeButton.onClick.AddListener(ResumeButtonAction);
        quitButton.onClick.AddListener(QuitButtonAction);
        restartButton.onClick.AddListener(RestartButtonAction);
        settingsButton.onClick.AddListener(SettingsButtonAction);
    }

    private void ResumeButtonAction()
    {
        MenuNavigationButtons.Instance.PlayButtonSound();
        GameStateManager.Instance.ResumePlayingState();
        MenuNavigationButtons.Instance.SwitchPanels(pauseScreenPanel, gameScreenPanel);
    }

    private void QuitButtonAction()
    {
        MenuNavigationButtons.Instance.PlayButtonSound();
        CleanupGameObjects();
        gameScreenPanel.SetActive(false);
        GameStateManager.Instance.ResetCurrentRun();
        MenuNavigationButtons.Instance.SwitchPanels(pauseScreenPanel, startScreenPanel);
        GameStateManager.Instance.EnterMenuState();
    }

    private void CleanupGameObjects()
    {
        // Destroy all debris
        DebrisMoveScript[] allDebris = FindObjectsByType<DebrisMoveScript>(FindObjectsSortMode.None);
        foreach (DebrisMoveScript debris in allDebris)
        {
            Destroy(debris.gameObject);
        }

        // Destroy all coins from last run
        CoinBehavior[] allCoins = FindObjectsByType<CoinBehavior>(FindObjectsSortMode.None);
        foreach (CoinBehavior coin in allCoins)
        {
            Destroy(coin.gameObject);
        }

        HealthUI[] healthUIs = FindObjectsByType<HealthUI>(FindObjectsSortMode.None);
        HealthUI healthUI = healthUIs.Length > 0 ? healthUIs[0] : null;
        if (healthUI != null)
        {
            Destroy(healthUI.gameObject);
        }
    }

    private void RestartButtonAction()
    {
        MenuNavigationButtons.Instance.PlayButtonSound();
        GameStateManager.Instance.EnterPlayingState();
        MenuNavigationButtons.Instance.SwitchPanels(pauseScreenPanel, gameScreenPanel);
    }

    private void SettingsButtonAction()
    {
        MenuNavigationButtons.Instance.PlayButtonSound();
        SettingsScreenController controller = settingsScreenPanel.GetComponent<SettingsScreenController>();
        controller.SetPreviousPanel(pauseScreenPanel);
        MenuNavigationButtons.Instance.SwitchPanels(pauseScreenPanel, settingsScreenPanel);
    }
}