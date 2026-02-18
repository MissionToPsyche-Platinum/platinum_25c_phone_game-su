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


    [Header(("Buttons"))]
    [SerializeField] private Button startGameButton;
    [SerializeField] private Button shopScreenButton;
    [SerializeField] private Button statsScreenButton;
    [SerializeField] private Button settingsScreenButton;

    private void Awake()
    {
        startGameButton.onClick.AddListener(StartGameButtonAction);
        shopScreenButton.onClick.AddListener(ShopScreenButtonAction);
        statsScreenButton.onClick.AddListener(StatsScreenButtonAction);
        settingsScreenButton.onClick.AddListener(SettingsScreenButtonAction);
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
        MenuNavigationButtons.Instance.SwitchPanels(startScreenPanel, settingsScreenPanel);

    }
}