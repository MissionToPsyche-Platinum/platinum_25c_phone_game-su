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
        // Reloads the scene to reset everything (consistent with your Pause Menu Restart)
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ShopButtonClicked()
    {
        this.gameObject.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        
        MenuNavigationButtons menuNav = FindFirstObjectByType<MenuNavigationButtons>();
        
        if (menuNav != null)
        {
            // Hide the game screen (since we are leaving the game)
            if (menuNav.gameScreenPanel != null) 
                menuNav.gameScreenPanel.SetActive(false);

            // Open the Shop
            if (menuNav.shopScreenPanel != null) 
                menuNav.shopScreenPanel.SetActive(true);
        }
    }

    public void Show()
    {
        this.gameObject.SetActive(true);
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
    }
}