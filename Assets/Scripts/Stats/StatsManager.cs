using System;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class StatsManager : MonoBehaviour
{
    [SerializeField] private ProbeCollisionHandler myProbeCollisionHandler;
    [SerializeField] private DebrisSpawnScript myDebrisSpawnScript;
    [SerializeField] private ScoreIncrement myScoreIncrement;
    
    public int coinsCollected = 0;
    public int asteroidsDodged = 0;
    public float highscore = 0;
    
    public static StatsManager instance { get; private set; }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        myProbeCollisionHandler.OnCoinCollected += OnCoinCollected;
        myProbeCollisionHandler.OnTakeDamage += OnTakeDamage;

        myDebrisSpawnScript.OnDebrisSpawned += OnDebrisSpawned;

        GameStateManager.Instance.OnStartPlaying += OnStartPlaying;
    }

    private void OnDestroy()
    {
        GameStateManager.Instance.OnStartPlaying -= OnStartPlaying;
    }

    private void OnStartPlaying(object sender, GameStateManager.GameStateChangeEventArgs e)
    {
        coinsCollected = 0;
        asteroidsDodged = 0;
    }

    private void OnDebrisSpawned(object sender, EventArgs e)
    {
        asteroidsDodged++;
    }

    private void OnTakeDamage(object sender, EventArgs e)
    {
        asteroidsDodged--;
        highscore = myScoreIncrement.GetCurrentScore();
    }

    private void OnCoinCollected(object sender, ProbeCollisionHandler.CoinCollectedEventArgs e)
    {
        coinsCollected += e.value;
    }
}