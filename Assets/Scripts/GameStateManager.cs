using System;
using System.Data;
using UnityEngine;
using System.Collections.Generic;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance { get; private set; }
    
    //Fields for other scripts to reference
    public enum GameState {
        None,
        MainMenu,
        Playing,
        Paused,
    }

    public enum GameStage
    {
        None,
        Earth,
        Moon,
        Mars,
        Psyche,
        Jupiter,
        Saturn, 
        Uranus,
        Neptune,
        ProximaCentauri,
        AlphaCentauriA,
        AlphaCentauriB,
        Transition
    }

    public GameState currentGameState = GameState.None;
    public GameStage gameStage = GameStage.None;
    public int totalCoins = 0;
    public int maxHealthLevel = 0;
    public int armorLevel = 0;
    public int speedLevel = 0;
    private int coinMultiplier = 1;

    public Vector3 startingProbePosition;
    public GameStage startingGameStage;
    public float startingScore;
    public int startingHealth;
    public double longestRealDistance = 0;
    public double TotalAsteroidsHit = 0;

    public int currentProbeSkin = 0;

    public bool moonUnlocked = false;
    public bool marsUnlocked = false;
    public bool psycheUnlocked = false;

    private bool[] checkpointAvailable = {true, false, false, false, false, false, false, false, false, false, false};
    private bool[] checkpointUnlocked = {true, false, false, false, false, false, false, false, false, false, false};
    private int[] checkpointUnlockCosts = {0, 150, 500, 1000, 2000, 3000, 4000, 5000, 7500, 10000, 12500};
    public int[] checkpointScores = {0, 300, 1000, 2000, 4000, 6000, 8000, 10000, 15000, 20000, 25000};

    private Vector3 probePosition;

    public class GameStateChangeEventArgs : EventArgs
    {
        public GameState PreviousState;
    }
    public event EventHandler<GameStateChangeEventArgs> OnStartPlaying;
    public event EventHandler<GameStateChangeEventArgs> OnStopPlaying;
    public event EventHandler<GameStateChangeEventArgs> OnPausePlaying;
    public event EventHandler<EventArgs> OnEnterMainMenu;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        currentGameState = GameState.MainMenu;
        gameStage = GameStage.Transition;
        totalCoins = 0;
    }

    public void UpdateProbePosition(Vector3 newPosition)
    {
        probePosition = newPosition;
    }

    public Vector3 GetProbePosition()
    {
        return probePosition;
    }

    public void UpdateAvailableCheckpoints(int score){
        int index = 0;
        while(index < checkpointScores.Length && checkpointScores[index] <= score){
            checkpointAvailable[index] = true;
            index++;
        }
    }

    public void EnterPlayingState()
    {
        startingHealth = 3 + ProbeUpgradeManager.Instance.GetHealthBonus();
        Time.timeScale = 1f;
        OnStartPlaying?.Invoke(this, new GameStateChangeEventArgs()
        {
            PreviousState = currentGameState
        });
        currentGameState = GameState.Playing;
    }

    public void EnterMenuState()
    {
        Time.timeScale = 0f;
        OnStopPlaying?.Invoke(this, new GameStateChangeEventArgs()
        {
            PreviousState = currentGameState
        });
        currentGameState = GameState.MainMenu;
        OnEnterMainMenu?.Invoke(this, EventArgs.Empty);
    }

    public void EnterPausedState()
    {
        Time.timeScale = 0f;
        OnPausePlaying?.Invoke(this, new GameStateChangeEventArgs()
        {
            PreviousState = currentGameState
        });
        currentGameState = GameState.Paused;
    }

    /// <summary>
    /// Method to enter playing state without invoking the OnStartPlaying event
    /// </summary>
    public void ResumePlayingState()
    {
        Time.timeScale = 1f;
        currentGameState = GameState.Playing;
    }

    public void AddCoins(int amount)
    {
        totalCoins += coinMultiplier * amount;
    }

    public void SetCoinMultiplier(int coinMultiplierVal)
    {
        coinMultiplier = coinMultiplierVal;
    }

    public int GetCoins()
    {
        return totalCoins;
    }

    public void SetGameStage(int stage)
    {
        switch(stage){
            case 0:
            gameStage = GameStage.None;
            break;
            case 1:
            gameStage = GameStage.Transition;
            break;
            case 2:
            gameStage = GameStage.Earth;
            break;
            case 3:
            gameStage = GameStage.Moon;
            break;
            case 4:
            gameStage = GameStage.Mars;
            break;
            case 5:
            gameStage = GameStage.Psyche;
            break;
            case 6:
            gameStage = GameStage.Jupiter;
            break;
            case 7:
            gameStage = GameStage.Saturn;
            break;
            case 8:
            gameStage = GameStage.Uranus;
            break;
            case 9:
            gameStage = GameStage.Neptune;
            break;
            case 10:
            gameStage = GameStage.ProximaCentauri;
            break;
            case 11:
            gameStage = GameStage.AlphaCentauriA;
            break;
            case 12:
            gameStage = GameStage.AlphaCentauriB;
            break;
            default:
            gameStage = GameStage.None;
            break;
        }
    }

    private int GameStageToInt(GameStage stage){
        switch(stage){
            case GameStage.None:
            return 0;
            case GameStage.Transition:
            return 1;
            case GameStage.Earth:
            return 2;
            case GameStage.Moon:
            return 3;
            case GameStage.Mars:
            return 4;
            case GameStage.Psyche:
            return 5;
            case GameStage.Jupiter:
            return 6;
            case GameStage.Saturn:
            return 7;
            case GameStage.Uranus:
            return 8;
            case GameStage.Neptune:
            return 9;
            case GameStage.ProximaCentauri:
            return 10;
            case GameStage.AlphaCentauriA:
            return 11;
            case GameStage.AlphaCentauriB:
            return 12;
            default:
            return 0;
        }
    }

    public int GetGameStageInt(){
        return GameStageToInt(gameStage);
    }

    public bool IsStageAvailable(GameStage stage){
        if(GameStageToInt(stage) - 2 < checkpointAvailable.Length)
        return checkpointAvailable[GameStageToInt(stage) - 2];
        return true;
    }

    public bool IsStageUnlocked(GameStage stage)
    {
        if(GameStageToInt(stage) - 2 < checkpointUnlocked.Length)
        return checkpointUnlocked[GameStageToInt(stage) - 2];
        return true;
    }

    public int GetStageUnlockCost(GameStage stage)
    {
        if(GameStageToInt(stage) - 2 < checkpointUnlocked.Length)
        return checkpointUnlockCosts[GameStageToInt(stage) - 2];
        return 0;
    }

    public bool TryUnlockStage(GameStage stage)
    {
        int cost = GetStageUnlockCost(stage);
        if (totalCoins < cost) return false;
        AddCoins(-cost);
        checkpointUnlocked[GameStageToInt(stage) - 2] = true;
        return true;
    }

    public void SetProbeSkin(int skinIndex)
    {
        currentProbeSkin = skinIndex;
    }

    public int GetProbeSkin()
    {
        return currentProbeSkin;
    }

    public void ResetCurrentRun()
    {
        gameStage = GameStage.Earth;
    }
}