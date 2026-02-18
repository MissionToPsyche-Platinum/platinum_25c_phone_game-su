using System;
using UnityEngine;
using UnityEngine.UI;

//Uses the GameStateManager and MenuNavManager to switch panels, and appropriately update the GameState variable
public class GameScreenButtonsController : MonoBehaviour
{
    [Header("Panels Can Switch To")]
    [SerializeField] private GameObject gameScreenPanel;
    [SerializeField] private GameObject pauseScreenPanel;
    
    [Header("Buttons")]
    [SerializeField] private Button pauseScreenButton;

    private void Awake()
    {
        pauseScreenButton.onClick.AddListener(PauseButtonAction);
    }

    private void PauseButtonAction()
    {
        MenuNavigationButtons.Instance.PlayButtonSound();
        GameStateManager.Instance.EnterPausedState();
        MenuNavigationButtons.Instance.SwitchPanels(gameScreenPanel, pauseScreenPanel);
    }
}