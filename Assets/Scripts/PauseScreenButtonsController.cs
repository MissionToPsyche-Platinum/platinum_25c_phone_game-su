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
        MenuNavigationButtons.Instance.SwitchPanels(pauseScreenPanel, startScreenPanel);
        GameStateManager.Instance.EnterMenuState();
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