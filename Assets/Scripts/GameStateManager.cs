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
    public event EventHandler<EventArgs> OnStartPlaying;

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

    public void AddCoins(int amount)
    {
        totalCoins += amount;
    }

    public int GetCoins()
    {
        return totalCoins;
    }
}