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

        // Ensure the visual panel is hidden at the start
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
        
        // Also ensure the main object is hidden if this script is on the root
        this.gameObject.SetActive(false);
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
        MenuNavigationButtons.Instance.SwitchPanels(gameOverPanel, shopPanel);
    }
}