using System;
using UnityEngine;
using UnityEngine.UI;
public class SettingsScreenButtonController : MonoBehaviour
{
    [Header("Panels Can Switch To")]
    [SerializeField] private GameObject settingsScreenPanel;
    [SerializeField] private GameObject startScreenPanel;

    [Header("Buttons")]
    [SerializeField] private Button openSettingsButton;

    private void Awake()
    {
        openSettingsButton.onClick.AddListener(OpenSettingsAction);
    }

    private void OpenSettingsAction()
    {
        MenuNavigationButtons.Instance.PlayButtonSound();
        SettingsScreenController controller = settingsScreenPanel.GetComponent<SettingsScreenController>();
        controller.SetPreviousPanel(startScreenPanel);
        MenuNavigationButtons.Instance.SwitchPanels(startScreenPanel, settingsScreenPanel);
    }
}