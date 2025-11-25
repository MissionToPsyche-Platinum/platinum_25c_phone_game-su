using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuNavigationButtons : MonoBehaviour
{
    [SerializeField] private Button settingsScreenButton;
    [SerializeField] private Button startScreenButton;
    [SerializeField] private Button gameScreenButton;

    private void Awake() {
        //enables all buttons that have been connected
        if (settingsScreenButton != null) {
            settingsScreenButton.onClick.AddListener(() => LoadScene("Settings Screen"));
        }
        if (startScreenButton) {
            startScreenButton.onClick.AddListener(() => LoadScene("Start Screen"));
        }
        if (gameScreenButton != null) {
            gameScreenButton.onClick.AddListener(() => LoadScene("Game Screen"));
        }
    }

    private void LoadScene(string newScene) {
        SceneManager.LoadScene(newScene);
    }
}
