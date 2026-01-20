using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuNavigationButtons : MonoBehaviour
{
    [SerializeField] private Button settingsScreenButton;
    [SerializeField] private Button startScreenButton;
    [SerializeField] private Button gameScreenButton;

    public GameObject StartScreenPanel;
    public GameObject GameScreenPanel;
    public GameObject SettingsScreenPanel;

    private void Awake() {
        //enables all buttons that have been connected
        if (settingsScreenButton != null) {
            settingsScreenButton.onClick.AddListener(() => SwitchPanels(StartScreenPanel, SettingsScreenPanel));
        }
        if (startScreenButton) {
            startScreenButton.onClick.AddListener(() => SwitchPanels(SettingsScreenPanel, StartScreenPanel));
        }
        if (gameScreenButton != null) {
            gameScreenButton.onClick.AddListener(() => SwitchPanels(StartScreenPanel, GameScreenPanel));
        }
    }

    void Start(){
        GameScreenPanel.SetActive(false);
        SettingsScreenPanel.SetActive(false);
    }

    private void LoadScene(string newScene) {
        SceneManager.LoadScene(newScene);
    }

    private void SwitchPanels(GameObject srcPanel, GameObject dstPanel){
        srcPanel.SetActive(false);
        dstPanel.SetActive(true);
    }
}
