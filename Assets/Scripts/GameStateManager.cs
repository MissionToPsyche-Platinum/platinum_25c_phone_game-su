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
        DeathScreen
    }

    public enum GameStage
    {
        None,
        Earth,
        Moon,
        Mars,
        Psyche
    }

    public GameState currentGameState = GameState.None;
    public GameStage gameStage = GameStage.None;
    public int totalCoins = 0;
    public int maxHealthLevel = 0;
    public int armorLevel = 0;
    public int speedLevel = 0;
    private int coinMultiplier = 1;

    private Vector3 probePosition;

    public event EventHandler<EventArgs> OnStartPlaying;
    public event EventHandler<EventArgs> OnStopPlaying;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        currentGameState = GameState.Menu;
        totalCoins = 1000;
    }

    public void StartPlaying()
    {
        OnStartPlaying?.Invoke(this, EventArgs.Empty);
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

    public void StopPlaying()
    {
        OnStopPlaying?.Invoke(this, EventArgs.Empty);
        currentGameState = GameState.Menu;
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
            gameStage = GameStage.Earth;
            break;
            case 2:
            gameStage = GameStage.Moon;
            break;
            case 3:
            gameStage = GameStage.Mars;
            break;
            case 4:
            gameStage = GameStage.Psyche;
            break;
            default:
            gameStage = GameStage.None;
            break;
        }
    }

    public int GetGameStage(){
        switch(gameStage){
            case GameStage.None:
            return 0;
            case GameStage.Earth:
            return 1;
            case GameStage.Moon:
            return 2;
            case GameStage.Mars:
            return 3;
            case GameStage.Psyche:
            return 4;
            default:
            return 0;
        }
    }
}