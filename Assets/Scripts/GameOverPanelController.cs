using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameOverPanelController : MonoBehaviour
{
    [SerializeField] private Button restartGameButton;
    [SerializeField] private GameObject gameOverPanel;

    private void Awake()
    {
        restartGameButton.onClick.AddListener(RestartGameButtonClicked);
        gameOverPanel.SetActive(false);
    }

    public void RestartGameButtonClicked()
    {
        gameOverPanel.SetActive(false);
        GameStateManager.Instance.StartPlaying();
    }

    public void Show()
    {
        this.gameObject.SetActive(true);
    }
}