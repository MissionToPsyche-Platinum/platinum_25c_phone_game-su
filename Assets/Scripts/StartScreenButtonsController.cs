using System;
using UnityEngine;
using UnityEngine.UI;

public class StartScreenButtonsController : MonoBehaviour
{
    [Header("Panels Can Switch To")]
    [SerializeField] private GameObject startScreenPanel;
    [SerializeField] private GameObject gameScreenPanel;
    [SerializeField] private GameObject shopScreenPanel;
    [SerializeField] private GameObject settingsScreenPanel;
    [SerializeField] private GameObject statsScreenPanel;
    [SerializeField] private GameObject componentsScreenPanel;


    [Header(("Buttons"))]
    [SerializeField] private Button startGameButton;
    [SerializeField] private Button shopScreenButton;
    [SerializeField] private Button statsScreenButton;
    [SerializeField] private Button settingsScreenButton;
    [SerializeField] private Button disclaimerButton;
    [SerializeField] private Button componentsScreenButton;

    [Header("Other")]
    [SerializeField] private DisclaimerPanelController disclaimerPanel;

    private void Awake()
    {
        startGameButton.onClick.AddListener(StartGameButtonAction);
        shopScreenButton.onClick.AddListener(ShopScreenButtonAction);
        statsScreenButton.onClick.AddListener(StatsScreenButtonAction);
        settingsScreenButton.onClick.AddListener(SettingsScreenButtonAction);
        disclaimerButton.onClick.AddListener(DisclaimerButtonAction);
        componentsScreenButton.onClick.AddListener(ComponentsScreenButtonAction);
    }

    private void StartGameButtonAction()
    {
        MenuNavigationButtons.Instance.PlayButtonSound();
        MenuNavigationButtons.Instance.SwitchPanels(startScreenPanel, gameScreenPanel);
        GameStateManager.Instance.EnterPlayingState();
    }

    private void ShopScreenButtonAction()
    {
        MenuNavigationButtons.Instance.PlayButtonSound();
        MenuNavigationButtons.Instance.SwitchPanels(startScreenPanel, shopScreenPanel);

    }

    private void StatsScreenButtonAction()
    {
        MenuNavigationButtons.Instance.PlayButtonSound();
        MenuNavigationButtons.Instance.SwitchPanels(startScreenPanel, statsScreenPanel);

    }

    private void SettingsScreenButtonAction()
    {
        MenuNavigationButtons.Instance.PlayButtonSound();
        SettingsScreenController controller = settingsScreenPanel.GetComponent<SettingsScreenController>();
        controller.SetPreviousPanel(startScreenPanel);
        MenuNavigationButtons.Instance.SwitchPanels(startScreenPanel, settingsScreenPanel);
    }

    private void ComponentsScreenButtonAction()
    {
        MenuNavigationButtons.Instance.PlayButtonSound();
        MenuNavigationButtons.Instance.SwitchPanels(startScreenPanel, componentsScreenPanel);
    }

    private void DisclaimerButtonAction()
    {
        disclaimerPanel.Show();
        startScreenPanel.SetActive(false);
    }
}