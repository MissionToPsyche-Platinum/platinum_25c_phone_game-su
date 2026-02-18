using UnityEngine;
using UnityEngine.UI;

public class ShopScreenButtonsController : MonoBehaviour
{
    [Header("Panels Can Switch To")]
    [SerializeField] private GameObject shopScreenPanel;
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
        MenuNavigationButtons.Instance.SwitchPanels(shopScreenPanel, startScreenPanel);
    }
}