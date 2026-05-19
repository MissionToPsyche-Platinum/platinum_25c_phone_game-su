using UnityEngine;
using UnityEngine.UI;

public class CodexBackButton : MonoBehaviour
{
    [Header("Panels Can Switch To")]
    [SerializeField] private GameObject codexScreenPanel;
    [SerializeField] private GameObject startScreenPanel;
    
    [Header("Buttons")]
    [SerializeField] private Button backToMainMenuButton;

    private void Awake()
    {
        backToMainMenuButton.onClick.AddListener(BackButtonAction);
    }

    private void BackButtonAction()
    {
        MenuNavigationButtons.Instance.PlayButtonSound();
        MenuNavigationButtons.Instance.SwitchPanels(codexScreenPanel, startScreenPanel);
    }
}