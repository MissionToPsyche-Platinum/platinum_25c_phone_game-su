using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuNavigationButtons : MonoBehaviour
{
    [SerializeField] private Button settingsScreenButton;
    [SerializeField] private Button startScreenButton;
    [SerializeField] private Button gameScreenButton;
    [SerializeField] private AudioClip buttonSoundClip;

    public GameObject startScreenPanel;
    public GameObject gameScreenPanel;
    public GameObject settingsScreenPanel;

    public GameObject checkpointSpawner;
    private CheckpointSpawnScript checkpointSpawnScript;

    private void Awake() {
        //enables all buttons that have been connected
        if (settingsScreenButton != null) {
            settingsScreenButton.onClick.AddListener(() => SettingsScreenButtonAction());
        }
        if (startScreenButton) {
            startScreenButton.onClick.AddListener(() => StartScreenButtonAction());
        }
        if (gameScreenButton != null) {
            gameScreenButton.onClick.AddListener(() => GameScreenButtonAction());
        }
    }

    void Start(){
        checkpointSpawnScript = checkpointSpawner.GetComponent<CheckpointSpawnScript>();
        
        gameScreenPanel.SetActive(false);
        settingsScreenPanel.SetActive(false);
    }

    private void StartScreenButtonAction(){
        SFXController.instance.PlaySoundFXClip(buttonSoundClip, transform, 1f);
        SwitchPanels(settingsScreenPanel, startScreenPanel);
    }

    private void GameScreenButtonAction(){
        SFXController.instance.PlaySoundFXClip(buttonSoundClip, transform, 1f);
        SwitchPanels(startScreenPanel, gameScreenPanel);
        GameStateManager.Instance.StartPlaying();
    }

    private void SettingsScreenButtonAction(){
        SFXController.instance.PlaySoundFXClip(buttonSoundClip, transform, 1f);
        SwitchPanels(startScreenPanel, settingsScreenPanel);
    }

    private void SwitchPanels(GameObject srcPanel, GameObject dstPanel){
        srcPanel.SetActive(false);
        dstPanel.SetActive(true);
    }
}