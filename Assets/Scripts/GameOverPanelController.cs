using System;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverPanelController : MonoBehaviour
{
    [SerializeField] private Button restartGameButton;
    [SerializeField] private Button shopButton;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject gameScreenPanel;
    [SerializeField] private GameObject shopPanel;

    private void Awake()
    {
        if (restartGameButton != null)
            restartGameButton.onClick.AddListener(RestartGameButtonClicked);

        if (shopButton != null)
            shopButton.onClick.AddListener(ShopButtonClicked);
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
}