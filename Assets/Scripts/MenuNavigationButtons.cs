using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuNavigationButtons : MonoBehaviour
{
    [SerializeField] private Button settingsScreenButton;
    [SerializeField] private Button startScreenButton;
    [SerializeField] private Button gameScreenButton;

    public GameObject startScreenPanel;
    public GameObject gameScreenPanel;
    public GameObject settingsScreenPanel;

    public GameObject checkpointSpawner;
    private CheckpointSpawnScript checkpointSpawnScript;

    private void Awake() {
        //enables all buttons that have been connected
        if (settingsScreenButton != null) {
            settingsScreenButton.onClick.AddListener(() => SwitchPanels(startScreenPanel, settingsScreenPanel));
        }
        if (startScreenButton) {
            startScreenButton.onClick.AddListener(() => SwitchPanels(settingsScreenPanel, startScreenPanel));
        }
        if (gameScreenButton != null) {
            gameScreenButton.onClick.AddListener(() => StartGame());
        }
    }

    void Start(){
        checkpointSpawnScript = checkpointSpawner.GetComponent<CheckpointSpawnScript>();

        gameScreenPanel.SetActive(false);
        settingsScreenPanel.SetActive(false);
    }

    private void LoadScene(string newScene) {
        SceneManager.LoadScene(newScene);
    }

    private void StartGame(){
        checkpointSpawnScript.resumeMovement(0);
        SwitchPanels(startScreenPanel, gameScreenPanel);
    }

    private void SwitchPanels(GameObject srcPanel, GameObject dstPanel){
        srcPanel.SetActive(false);
        dstPanel.SetActive(true);
    }
}
