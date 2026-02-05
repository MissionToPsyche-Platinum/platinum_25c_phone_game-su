using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuNavigationButtons : MonoBehaviour
{
    [SerializeField] private Button settingsScreenButton;
    [SerializeField] private Button startScreenButton;
    [SerializeField] private Button gameScreenButton;
    [SerializeField] private Button shopScreenButton;
    [SerializeField] private Button statsScreenButton;
    [SerializeField] private Button backFromShopButton;
    [SerializeField] private Button backFromStatsButton;
    [SerializeField] private AudioClip buttonSoundClip;

    public GameObject startScreenPanel;
    public GameObject gameScreenPanel;
    public GameObject settingsScreenPanel;
    public GameObject shopScreenPanel;
    public GameObject statsScreenPanel;
    public GameObject checkpointSpawner;

    private CheckpointSpawnScript checkpointSpawnScript;

    private void Awake()
    {
        //enables all buttons that have been connected
        if (settingsScreenButton != null)
        {
            settingsScreenButton.onClick.AddListener(() => SettingsScreenButtonAction());
        }
        if (startScreenButton)
        {
            startScreenButton.onClick.AddListener(() => StartScreenButtonAction());
        }
        if (gameScreenButton != null)
        {
            gameScreenButton.onClick.AddListener(() => GameScreenButtonAction());
        }
        if (shopScreenButton != null)
        {
            shopScreenButton.onClick.AddListener(() => ShopScreenButtonAction());
        }
        if (backFromShopButton != null)
        {
            backFromShopButton.onClick.AddListener(() => BackFromShopAction());
        }
        if (statsScreenButton != null)
        {
            statsScreenButton.onClick.AddListener((() => StatsScreenButtonAction()));
        }

        if (backFromStatsButton != null)
        {
            backFromStatsButton.onClick.AddListener((() => BackFromStatsAction()));
        }
    }

    void Start()
    {
        checkpointSpawnScript = checkpointSpawner.GetComponent<CheckpointSpawnScript>();

        gameScreenPanel.SetActive(false);
        settingsScreenPanel.SetActive(false);
        shopScreenPanel.SetActive(false);
    }

    private void StatsScreenButtonAction()
    {
        SFXController.instance.PlaySoundFXClip(buttonSoundClip, transform, 1f);
        SwitchPanels(startScreenPanel, statsScreenPanel);
    }

    private void BackFromStatsAction()
    {
        SFXController.instance.PlaySoundFXClip(buttonSoundClip, transform, 1f);
        SwitchPanels(statsScreenPanel, startScreenPanel);
    }
    
    private void StartScreenButtonAction()
    {
        SFXController.instance.PlaySoundFXClip(buttonSoundClip, transform, 1f);
        SwitchPanels(settingsScreenPanel, startScreenPanel);
    }

    private void GameScreenButtonAction()
    {
        SFXController.instance.PlaySoundFXClip(buttonSoundClip, transform, 1f);
        SwitchPanels(startScreenPanel, gameScreenPanel);
        GameStateManager.Instance.StartPlaying();
    }

    private void SettingsScreenButtonAction()
    {
        SFXController.instance.PlaySoundFXClip(buttonSoundClip, transform, 1f);
        SwitchPanels(startScreenPanel, settingsScreenPanel);
    }

    private void ShopScreenButtonAction()
    {
        SFXController.instance.PlaySoundFXClip(buttonSoundClip, transform, 1f);
        SwitchPanels(startScreenPanel, shopScreenPanel);
    }

    private void BackFromShopAction()
    {
        SFXController.instance.PlaySoundFXClip(buttonSoundClip, transform, 1f);
        SwitchPanels(shopScreenPanel, startScreenPanel);
    }

    private void SwitchPanels(GameObject srcPanel, GameObject dstPanel)
    {
        srcPanel.SetActive(false);
        dstPanel.SetActive(true);
    }
}