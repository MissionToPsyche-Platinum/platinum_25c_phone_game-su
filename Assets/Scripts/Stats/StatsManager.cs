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

    private void OnCoinCollected(object sender, EventArgs e)
    {
        coinsCollected++;
    }
}