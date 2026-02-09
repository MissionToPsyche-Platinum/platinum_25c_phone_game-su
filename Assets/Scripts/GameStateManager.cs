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
            case 1:
            gameStage = GameStage.Earth;
            case 2:
            gameStage = GameStage.Moon;
            case 3:
            gameStage = GameStage.Mars;
            case 4:
            gameStage = GameStage.Psyche;
            default:
            gameStage = GameStage.None;
        }
    }

    public int GetGameStage(){
        switch(gameStage){
            case None:
            return 0;
            case Earth:
            return 1;
            case Moon:
            return 2;
            case Mars:
            return 3;
            case Psyche:
            return 4;
            default:
            return 0;
        }
    }
}