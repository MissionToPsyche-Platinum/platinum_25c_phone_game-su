using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class CheckpointSpawnScript : MonoBehaviour
{

    [SerializeField] private GameObject scoreIncrement;
    [SerializeField] private GameObject debrisSpawner;
    [SerializeField] private GameObject coinSpawner;
    [SerializeField] private GameObject powerUpSpawner;

    public GameObject[] checkpoints;
    public int[] checkpointScores;
    public string[] checkpointMessages;

    [SerializeField] private TMP_Text messageText;

    [SerializeField] private GameObject moonArrivalPopupPrefab;
    [SerializeField] private GameObject marsArrivalPopupPrefab;
    [SerializeField] private GameObject psycheArrivalPopupPrefab;
    [SerializeField] private GameObject neptuneArrivalPopupPrefab;
    [SerializeField] private GameObject proximaCentauriArrivalPopupPrefab;
    [SerializeField] private GameObject alphaCentauriAArrivalPopupPrefab;
    [SerializeField] private GameObject alphaCentauriBAArrivalPopupPrefab;
    private GameObject[] checkpointInstances;

    private const int numCheckpoints = 11;
    private float checkpointZ = 0.9f; //z-coord of checkpoints for layering with other visuals

    private int nextCheckpoint = 0;
    private int startingCheckpoint;

    private ScoreIncrement scoreSystem;
    private DebrisSpawnScript debrisSpawnSystem;
    private CoinSpawner coinSpawnSystem;
    private PowerUpSpawnScript powerUpSpawnSystem;

    private bool stopped = false;
    private int pendingPopupIndex = -1;

    //for spawners, when the checkpoint is on or off screen
    public event EventHandler<EventArgs> OnCheckpointReached;
    public event EventHandler<EventArgs> OnCheckpointPassed;

    //for movement of background elements, when the actual checkpoint stops moving
    public event EventHandler<EventArgs> OnCheckpointStopped;
    public event EventHandler<EventArgs> OnCheckpointResumed;

    private void Awake()
    {
        checkpointInstances = new GameObject[numCheckpoints];
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        scoreSystem = scoreIncrement.GetComponent<ScoreIncrement>();
        debrisSpawnSystem = debrisSpawner.GetComponent<DebrisSpawnScript>();
        coinSpawnSystem = coinSpawner.GetComponent<CoinSpawner>();
        powerUpSpawnSystem = powerUpSpawner.GetComponent<PowerUpSpawnScript>();
        nextCheckpoint = (int)GameStateManager.Instance.startingGameStage - 1;

        spawnCheckpoint(nextCheckpoint, true);

        GameStateManager.Instance.OnStartPlaying += OnStartPlaying;
        GameStateManager.Instance.OnStopPlaying += OnStopPlaying;
        GameStateManager.Instance.OnEnterMainMenu += GameStateManager_OnEnterMainMenu;
    }

    private void GameStateManager_OnEnterMainMenu(object sender, EventArgs e)
    {
        spawnCheckpoint((int)GameStateManager.Instance.startingGameStage - 1, true);
    }

    public void IncrementStartingCheckpoint(){
        if(GameStateManager.Instance.currentGameState == GameStateManager.GameState.MainMenu){
            startingCheckpoint = (startingCheckpoint + 1) % numCheckpoints;
            nextCheckpoint = startingCheckpoint;
            KillAllActiveCheckpoints();
            spawnCheckpoint(nextCheckpoint, true);
        }
    }

    public void DecrementStartingCheckpoint(){
        if(GameStateManager.Instance.currentGameState == GameStateManager.GameState.MainMenu){
            startingCheckpoint = startingCheckpoint > 0 ? startingCheckpoint - 1 : numCheckpoints - 1;
            nextCheckpoint = startingCheckpoint;
            KillAllActiveCheckpoints();
            spawnCheckpoint(nextCheckpoint, true);
        }
    }

    private void OnDestroy()
    {
        GameStateManager.Instance.OnStopPlaying -= OnStopPlaying;
        GameStateManager.Instance.OnStartPlaying -= OnStartPlaying;
    }

    public void spawnCheckpoint(int index, bool spawnAtCenter = false){
        Debug.Log("Spawn checkpoint: " + index);
        Vector3 spawnPos = new Vector3(0.0f, 0.00f, checkpointZ);
        if(!spawnAtCenter){
            //if the checkpoint being spawned isn't the initial one
            spawnPos.Set(0.0f, 8.0f, checkpointZ);
        }
        checkpointInstances[index] = Instantiate(checkpoints[index], spawnPos, Quaternion.identity);
        checkpointInstances[index].transform.localScale = new Vector3(0.15f, 0.15f, 0.15f);
        int capturedIndex = index;
        
        nextCheckpoint += 1;
    }

    public int GetCheckpointScore(int index){
        return checkpointScores[index];
    }

    public void HandleCheckpointExited()
    {
        OnCheckpointPassed?.Invoke(this, EventArgs.Empty);
    }

    public void resumeMovement(int index){
        GameStateManager.Instance.SetGameStage(index + 2);
        OnCheckpointResumed?.Invoke(this, EventArgs.Empty);
        HideMessage();
    }

    private void ShowMessage(int index)
    {
        if (messageText == null || index >= checkpointMessages.Length) return;
        messageText.text = checkpointMessages[index];
        StopAllCoroutines();
        StartCoroutine(FadeText(0f, 1f));
    }

    private void HideMessage()
    {
        if (messageText == null) return;
        StopAllCoroutines();
        StartCoroutine(FadeText(messageText.alpha, 0f));
    }

    private IEnumerator FadeText(float from, float to)
    {
        messageText.gameObject.SetActive(true);
        float elapsed = 0f;
        while (elapsed < 2f)
        {
            elapsed += Time.deltaTime;
            messageText.alpha = Mathf.Lerp(from, to, elapsed / 2f);
            yield return null;
        }
        messageText.alpha = to;
        if (to == 0f) messageText.gameObject.SetActive(false);
    }
    private void OnStartPlaying(object sender, GameStateManager.GameStateChangeEventArgs e)
    {
        KillAllActiveCheckpoints();
        
        //Setting default stuff for spawn logic
        nextCheckpoint = startingCheckpoint;
        stopped = false;
        
        //Spawn the new checkpoint and make it move
        spawnCheckpoint(nextCheckpoint, true);
        CheckpointMoveScript moveScript = checkpointInstances[startingCheckpoint].GetComponent<CheckpointMoveScript>();
        moveScript.SetAlreadyStoppedAtCenter(true);
        resumeMovement(startingCheckpoint);
    }
    
    private void OnStopPlaying(object sender, GameStateManager.GameStateChangeEventArgs e)
    {
        KillAllActiveCheckpoints();
    }
    
    private GameObject GetPopupForCheckpoint(int index) => index switch
    {
        1 => moonArrivalPopupPrefab,
        2 => marsArrivalPopupPrefab,
        3 => psycheArrivalPopupPrefab,
        7 => neptuneArrivalPopupPrefab,
        8 => proximaCentauriArrivalPopupPrefab,
        9 => alphaCentauriAArrivalPopupPrefab,
        10 => alphaCentauriBAArrivalPopupPrefab,
        _ => null
    };

    void Update()
    {
        if (GameStateManager.Instance.currentGameState != GameStateManager.GameState.Playing)
        {
            //If the game isn't playing, then ignore this stuff
            return;
        }
        
        float currentScore = scoreSystem.GetCurrentScore();
        if(nextCheckpoint < numCheckpoints && currentScore >= checkpointScores[nextCheckpoint]){
            int arrivedIndex = nextCheckpoint;
            OnCheckpointReached?.Invoke(this, EventArgs.Empty);
            spawnCheckpoint(nextCheckpoint);
            stopped = true;

            if (GetPopupForCheckpoint(arrivedIndex) != null)
            {
                pendingPopupIndex = arrivedIndex;
            }
            else
            {
                ShowMessage(arrivedIndex);
            }
        }
    }

    private void KillAllActiveCheckpoints()
    {
        foreach (GameObject checkpoint in checkpointInstances)
        {
            if (checkpoint != null)
            {
                Destroy(checkpoint);
            }
        }
    }
    
    public int GetNextCheckpointScore()
    {
        if (nextCheckpoint >= numCheckpoints)
        {
            //There are no more checkpoints
            return 99999999; //arbitrary impossible high number to reach, maybe change later
        }
        return checkpointScores[nextCheckpoint];
    }

    public int GetLastCheckpointScore()
    {
        if (nextCheckpoint - 1 <= 0)
        {
            //if the last checkpoint was earth or didn't exist
            return 0;
        }
        return checkpointScores[nextCheckpoint - 1];
    }

    public void pauseMovement(){
        OnCheckpointStopped?.Invoke(this, EventArgs.Empty);

        if (pendingPopupIndex >= 0)
        {
            int arrivedIndex = pendingPopupIndex;
            pendingPopupIndex = -1;
            GameObject popupInstance = PopupManager.Instance.DisplayFullScreenPopup(GetPopupForCheckpoint(arrivedIndex));
            CheckpointIntroPopup intro = popupInstance.GetComponent<CheckpointIntroPopup>();
            if (intro != null)
            {
                intro.OnDismissed += () => resumeMovement(arrivedIndex);
            }
        }
        else
        {
            StartCoroutine(AutoResumeAfterMessage());
        }
    }

    private IEnumerator AutoResumeAfterMessage()
    {
        yield return new WaitForSeconds(2.5f);
        if (GameStateManager.Instance.currentGameState == GameStateManager.GameState.Playing)
        {
            resumeMovement(nextCheckpoint - 1);
        }
    }
}