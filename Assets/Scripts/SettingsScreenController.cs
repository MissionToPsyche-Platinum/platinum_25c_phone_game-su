using UnityEngine;
using UnityEngine.UI;

public class SettingsScreenController : MonoBehaviour
{
    [SerializeField] private GameObject settingsScreenPanel;
    [SerializeField] private Button backButton;

    private GameObject previousPanel;

    private void Awake()
    {
        backButton.onClick.AddListener(BackButtonAction);
    }

    public void SetPreviousPanel(GameObject panel)
    {
        previousPanel = panel;
    }

    private void BackButtonAction()
    {
        MenuNavigationButtons.Instance.PlayButtonSound();
        if (previousPanel != null)
        {
            MenuNavigationButtons.Instance.SwitchPanels(settingsScreenPanel, previousPanel);
        }
    }
}