using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PauseMenuManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject pauseButton;
    [SerializeField] private GameObject pauseMenuPanel;
    [SerializeField] private GameObject resumeButton;
    [SerializeField] private GameObject quitButton;

    private bool isPaused = false;
    private PlayerInput playerInput;

    void Start()
    {
        // Initialize UI state
        if (pauseButton != null)
            pauseButton.SetActive(true);

        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(false);

        // Make sure game is running
        Time.timeScale = 1f;
        isPaused = false;

        // Get PlayerInput component if it exists
        playerInput = FindFirstObjectByType<PlayerInput>();
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f; // Freeze game time

        if (pauseButton != null)
            pauseButton.SetActive(false);

        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(true);

        // Disable gameplay input while paused
        if (playerInput != null)
        {
            playerInput.DeactivateInput();
        }
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f; // Resume game time

        if (pauseButton != null)
            pauseButton.SetActive(true);

        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(false);

        // Re-enable gameplay input
        if (playerInput != null)
        {
            playerInput.ActivateInput();
        }
    }

    public void QuitGame()
    {
        Time.timeScale = 1f; // Reset time scale before quitting

        // Re-enable input before quitting
        if (playerInput != null)
        {
            playerInput.ActivateInput();
        }

        // Takes you back to the game screen (change later if main menu screen is added)
        GameStateManager.Instance.StopPlaying();
   }

    // Optional: Handle pause input action (for keyboard/gamepad)
    public void OnPause(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }
}