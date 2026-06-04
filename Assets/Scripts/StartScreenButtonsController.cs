using System;
using UnityEngine;
using UnityEngine.UI;

public class StartScreenButtonsController : MonoBehaviour
{
    [Header("Panels Can Switch To")]
    [SerializeField] private GameObject startScreenPanel;
    [SerializeField] private GameObject gameScreenPanel;
    [SerializeField] private GameObject shopScreenPanel;
    [SerializeField] private GameObject settingsScreenPanel;
    [SerializeField] private GameObject statsScreenPanel;
    [SerializeField] private GameObject codexScreenPanel;
    [SerializeField] private GameObject creditsScreenPanel;
    [SerializeField] private GameObject moreSection;

    [Header(("Buttons"))]
    [SerializeField] private Button startGameButton;
    [SerializeField] private Button shopScreenButton;
    [SerializeField] private Button statsScreenButton;
    [SerializeField] private Button settingsScreenButton;
    [SerializeField] private Button codexScreenButton;
    [SerializeField] private Button moreButton;
    [SerializeField] private Button creditsScreenButton;

    [Header("Debug")]
    [Tooltip("HoldButton component on the same GameObject as the About/Disclaimer button.")]
    [SerializeField] private HoldButton aboutHoldButton;
    [Tooltip("The bug-icon / debug button revealed after holding About for 3 seconds. Starts hidden.")]
    [SerializeField] private Button debugButton;
    [Tooltip("The debug menu panel opened by the debug button. Starts hidden.")]
    [SerializeField] private DebugMenuController debugMenuController;

    private void Awake()
    {
        startGameButton.onClick.AddListener(StartGameButtonAction);
        shopScreenButton.onClick.AddListener(ShopScreenButtonAction);
        statsScreenButton.onClick.AddListener(StatsScreenButtonAction);
        settingsScreenButton.onClick.AddListener(SettingsScreenButtonAction);
        codexScreenButton.onClick.AddListener(CodexScreenButtonAction);
        moreButton.onClick.AddListener(MoreButtonAction);
        creditsScreenButton.onClick.AddListener(CreditsScreenButtonAction);

        // Debug button is hidden until unlocked
        if (debugButton != null)
        {
            debugButton.gameObject.SetActive(false);
            debugButton.onClick.AddListener(DebugButtonAction);
        }

        // Wire up the hold-to-unlock behaviour on the About button
        if (aboutHoldButton != null)
            aboutHoldButton.OnHoldComplete += OnAboutButtonHeld;
    }

    private void Start()
    {
        // If debug was already unlocked this session (e.g. scene reloaded), restore visibility.
        if (DebugManager.Instance != null && DebugManager.Instance.IsDebugUnlocked && debugButton != null)
            debugButton.gameObject.SetActive(true);

        if (DebugManager.Instance != null)
            DebugManager.Instance.OnDebugUnlocked += OnDebugUnlocked;
    }

    private void OnDestroy()
    {
        if (aboutHoldButton != null)
            aboutHoldButton.OnHoldComplete -= OnAboutButtonHeld;

        if (DebugManager.Instance != null)
            DebugManager.Instance.OnDebugUnlocked -= OnDebugUnlocked;
    }

    // ── Normal buttons ───────────────────────────────────────────────────────

    private void StartGameButtonAction()
    {
        MenuNavigationButtons.Instance.PlayButtonSound();
        MenuNavigationButtons.Instance.SwitchPanels(startScreenPanel, gameScreenPanel);
        GameStateManager.Instance.EnterPlayingState();
    }

    private void ShopScreenButtonAction()
    {
        MenuNavigationButtons.Instance.PlayButtonSound();
        MenuNavigationButtons.Instance.SwitchPanels(startScreenPanel, shopScreenPanel);
    }

    private void StatsScreenButtonAction()
    {
        MenuNavigationButtons.Instance.PlayButtonSound();
        MenuNavigationButtons.Instance.SwitchPanels(startScreenPanel, statsScreenPanel);
    }

    private void SettingsScreenButtonAction()
    {
        MenuNavigationButtons.Instance.PlayButtonSound();
        SettingsScreenController controller = settingsScreenPanel.GetComponent<SettingsScreenController>();
        controller.SetPreviousPanel(startScreenPanel);
        MenuNavigationButtons.Instance.SwitchPanels(startScreenPanel, settingsScreenPanel);
    }

    private void CodexScreenButtonAction(){
        MenuNavigationButtons.Instance.PlayButtonSound();
        SettingsScreenController controller = settingsScreenPanel.GetComponent<SettingsScreenController>();
        controller.SetPreviousPanel(startScreenPanel);
        MenuNavigationButtons.Instance.SwitchPanels(startScreenPanel, codexScreenPanel);
    }
    
    private void CreditsScreenButtonAction(){
        MenuNavigationButtons.Instance.PlayButtonSound();
        SettingsScreenController controller = settingsScreenPanel.GetComponent<SettingsScreenController>();
        controller.SetPreviousPanel(startScreenPanel);
        MenuNavigationButtons.Instance.SwitchPanels(startScreenPanel, creditsScreenPanel);
    }
    
    private void MoreButtonAction()
    {
        moreSection.SetActive(!moreSection.activeSelf);
    }

    // ── Debug unlock ─────────────────────────────────────────────────────────

    private void OnAboutButtonHeld()
    {
        if (DebugManager.Instance != null)
            DebugManager.Instance.UnlockDebugMode();
    }

    private void OnDebugUnlocked()
    {
        if (debugButton != null)
            debugButton.gameObject.SetActive(true);
    }

    private void DebugButtonAction()
    {
        if (debugMenuController != null)
            debugMenuController.Show();
    }
}