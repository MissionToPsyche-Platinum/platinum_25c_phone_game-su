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
        Earth
    }

    public GameState currentGameState = GameState.None;
    public GameStage gameStage = GameStage.None;
    public int totalCoins = 0;
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
}