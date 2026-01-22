using System;
using System.Linq;
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
        gameScreenPanel.SetActive(false);
        settingsScreenPanel.SetActive(false);
    }

    private void StartGame(){
        CallOnStartGameOnObjects();
        SwitchPanels(startScreenPanel, gameScreenPanel);
    }

    private void SwitchPanels(GameObject srcPanel, GameObject dstPanel){
        srcPanel.SetActive(false);
        dstPanel.SetActive(true);
    }

    private void CallOnStartGameOnObjects()
    {
        IListenToStartGame[] allGOsThatListenToStartGame = FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None).OfType<IListenToStartGame>().ToArray();
        foreach (IListenToStartGame objectListeningToStartGame in allGOsThatListenToStartGame)
        {
            objectListeningToStartGame.OnStartGame();
        }
    }
}