using System;
using System.Data;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance { get; private set; }
    
    //Fields for other scripts to reference
    public enum GameState {
        None,
        Menu,
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
        Transition
    }

    public GameState currentGameState = GameState.None;
    public GameStage gameStage = GameStage.None;
    public int totalCoins = 0;
    public int maxHealthLevel = 0;
    public int armorLevel = 0;
    public int speedLevel = 0;
    private int coinMultiplier = 1;

    private Vector3 probePosition;

    public class GameStateChangeEventArgs : EventArgs
    {
        public GameState PreviousState;
    }
    public event EventHandler<GameStateChangeEventArgs> OnStartPlaying;
    public event EventHandler<GameStateChangeEventArgs> OnStopPlaying;
    public event EventHandler<GameStateChangeEventArgs> OnPausePlaying;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        currentGameState = GameState.Menu;
        gameStage = GameStage.Transition;
        totalCoins = 1000;
    }

    public void EnterPlayingState()
    {
        Time.timeScale = 1f;
        OnStartPlaying?.Invoke(this, new GameStateChangeEventArgs()
        {
            PreviousState = currentGameState
        });
        currentGameState = GameState.Playing;
    }

    public void UpdateProbePosition(Vector3 newPosition)
    {
        probePosition = newPosition;
    }

    public Vector3 GetProbePosition()
    {
        return probePosition;
    }

    public void EnterMenuState()
    {
        Time.timeScale = 0f;
        OnStopPlaying?.Invoke(this, new GameStateChangeEventArgs()
        {
            PreviousState = currentGameState
        });
        currentGameState = GameState.Menu;
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
            default:
            gameStage = GameStage.None;
            break;
        }
    }

    public int GetGameStageInt(){
        switch(gameStage){
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
            default:
            return 0;
        }
    }
}