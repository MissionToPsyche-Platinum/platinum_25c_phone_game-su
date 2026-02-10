using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class MenuNavigationButtons : MonoBehaviour
{
    [Header("Main Menu Buttons")]
    [SerializeField] private Button settingsScreenButton;
    [SerializeField] private Button startScreenButton; 
    [SerializeField] private Button gameScreenButton;
    [SerializeField] private Button shopScreenButton;
    [SerializeField] private Button statsScreenButton;
    [SerializeField] private Button backFromShopButton;
    [SerializeField] private Button backFromStatsButton;
    [SerializeField] private Button backFromSettingsButton;
    
    [Header("Pause Menu Buttons")]
    [SerializeField] private Button pauseButton;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Button restartButton; // Added Restart Button

    [Header("Audio")]
    [SerializeField] private AudioClip buttonSoundClip;

    [Header("Panels")]
    public GameObject startScreenPanel;
    public GameObject gameScreenPanel;
    public GameObject settingsScreenPanel;
    public GameObject shopScreenPanel;
    public GameObject statsScreenPanel;
    public GameObject pauseMenuPanel;
    public GameObject checkpointSpawner;

    private CheckpointSpawnScript checkpointSpawnScript;
    private PlayerInput playerInput;
    private bool isPaused = false;

    private void Awake()
    {
        if (settingsScreenButton != null)
            settingsScreenButton.onClick.AddListener(() => SettingsScreenButtonAction());
        
        if (startScreenButton)
            startScreenButton.onClick.AddListener(() => StartScreenButtonAction());
        
        if (gameScreenButton != null)
            gameScreenButton.onClick.AddListener(() => GameScreenButtonAction());
        
        if (shopScreenButton != null)
            shopScreenButton.onClick.AddListener(() => ShopScreenButtonAction());
        
        if (backFromShopButton != null)
            backFromShopButton.onClick.AddListener(() => BackFromShopAction());
        
        if (statsScreenButton != null)
            statsScreenButton.onClick.AddListener((() => StatsScreenButtonAction()));

        if (backFromStatsButton != null)
            backFromStatsButton.onClick.AddListener((() => BackFromStatsAction()));

        if (backFromSettingsButton != null)
            backFromSettingsButton.onClick.AddListener(() => BackFromSettingsAction());

        if (pauseButton != null)
            pauseButton.onClick.AddListener(PauseGame);

        if (resumeButton != null)
            resumeButton.onClick.AddListener(ResumeGame);

        if (quitButton != null)
            quitButton.onClick.AddListener(QuitGame);

        // Hook up Restart to do the exact same thing as Quit
        if (restartButton != null)
            restartButton.onClick.AddListener(QuitGame);
    }

    void Start()
    {
        if (checkpointSpawner != null)
            checkpointSpawnScript = checkpointSpawner.GetComponent<CheckpointSpawnScript>();

        if (startScreenPanel != null) startScreenPanel.SetActive(true);
        if (gameScreenPanel != null) gameScreenPanel.SetActive(false);
        if (settingsScreenPanel != null) settingsScreenPanel.SetActive(false);
        if (shopScreenPanel != null) shopScreenPanel.SetActive(false);
        if (statsScreenPanel != null) statsScreenPanel.SetActive(false);
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);

        SetMainMenuButtonsActive(true);

        if (pauseButton != null) pauseButton.gameObject.SetActive(false);

        Time.timeScale = 1f;
        isPaused = false;

        playerInput = FindFirstObjectByType<PlayerInput>();
    }

    private void SetMainMenuButtonsActive(bool isActive)
    {
        if (gameScreenButton != null) gameScreenButton.gameObject.SetActive(isActive);
        if (settingsScreenButton != null) settingsScreenButton.gameObject.SetActive(isActive);
        if (shopScreenButton != null) shopScreenButton.gameObject.SetActive(isActive);
        if (statsScreenButton != null) statsScreenButton.gameObject.SetActive(isActive);
    }

    private void GameScreenButtonAction()
    {
        if (startScreenPanel != null) startScreenPanel.SetActive(false);
        
        SetMainMenuButtonsActive(false);

        if (gameScreenPanel != null) gameScreenPanel.SetActive(true);

        PlayButtonSound();

        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.StartPlaying();
        }
        
        if (pauseButton != null) pauseButton.gameObject.SetActive(true);
    }

    private void PlayButtonSound()
    {
        if (SFXController.instance != null && buttonSoundClip != null)
        {
            SFXController.instance.PlaySoundFXClip(buttonSoundClip, transform, 1f);
        }
    }

    private void PauseGame()
    {
        PlayButtonSound();

        isPaused = true;
        Time.timeScale = 0f;

        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(true);

        if (playerInput != null)
            playerInput.DeactivateInput();
    }

    private void ResumeGame()
    {
        PlayButtonSound();

        isPaused = false;
        Time.timeScale = 1f;

        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(false);
        
        if (playerInput != null)
            playerInput.ActivateInput();
    }

    // This method now handles both Quit and Restart buttons
    private void QuitGame()
    {
        PlayButtonSound();

        Time.timeScale = 1f;

        if (playerInput != null)
            playerInput.ActivateInput();

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

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

    private void StatsScreenButtonAction()
    {
        PlayButtonSound();
        SetMainMenuButtonsActive(false);
        SwitchPanels(startScreenPanel, statsScreenPanel);
    }

    private void BackFromStatsAction()
    {
        PlayButtonSound();
        SwitchPanels(statsScreenPanel, startScreenPanel);
        SetMainMenuButtonsActive(true);
    }
    
    private void StartScreenButtonAction()
    {
        PlayButtonSound();
        SwitchPanels(settingsScreenPanel, startScreenPanel);
        SetMainMenuButtonsActive(true);
    }

    private void SettingsScreenButtonAction()
    {
        PlayButtonSound();
        SetMainMenuButtonsActive(false);
        SwitchPanels(startScreenPanel, settingsScreenPanel);
    }

    private void BackFromSettingsAction()
    {
        PlayButtonSound();
        SwitchPanels(settingsScreenPanel, startScreenPanel);
        SetMainMenuButtonsActive(true);
    }

    private void ShopScreenButtonAction()
    {
        PlayButtonSound();
        SetMainMenuButtonsActive(false);
        SwitchPanels(startScreenPanel, shopScreenPanel);
    }

    private void BackFromShopAction()
    {
        PlayButtonSound();
        SwitchPanels(shopScreenPanel, startScreenPanel);
        SetMainMenuButtonsActive(true);
    }

    private void SwitchPanels(GameObject srcPanel, GameObject dstPanel)
    {
        if(srcPanel != null) srcPanel.SetActive(false);
        if(dstPanel != null) dstPanel.SetActive(true);
    }
}