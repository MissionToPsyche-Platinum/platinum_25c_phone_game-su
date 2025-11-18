using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuNavigationButtons : MonoBehaviour
{
    [SerializeField] private Button settingsScreenButton;
    [SerializeField] private Button startScreenButton;
    [SerializeField] private Button levelSelectionScreenButton;

    private void Awake() {
        //enables all buttons that have been connected
        if (settingsScreenButton != null) {
            settingsScreenButton.onClick.AddListener(() => LoadScene("Settings Screen"));
        }
        if (startScreenButton) {
            startScreenButton.onClick.AddListener(() => LoadScene("Start Screen"));
        }
        if (levelSelectionScreenButton != null) {
            levelSelectionScreenButton.onClick.AddListener(() => LoadScene("Level Selection Screen"));
        }
    }

    private void LoadScene(string newScene) {
        SceneManager.LoadScene(newScene);
    }
}
