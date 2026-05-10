using UnityEngine;
using System;

public class ProbeUpgradeManager : MonoBehaviour
{
    public static ProbeUpgradeManager Instance { get; private set; }

    public static readonly string[] Names =
    {
        "Speed", "Health", "Armor"
    };

    public const int MaxLevel = 5;

    private int[] _levels = new int[3];

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        for(int i = 0; i < 3; i++){
            _levels[i] = 0;
        }
    }

    public int GetLevel(int i) => _levels[i];
    public bool IsMaxed(int i) => _levels[i] >= MaxLevel;
    public int GetUpgradeCost(int i) => 10 * (int)Math.Pow(2, _levels[i]);

    public float GetSpeedMultiplier() => Mathf.Pow(1.2f, _levels[0]);
    public int GetHealthBonus() => _levels[1];
    public int GetDodgeChance() => _levels[2] * 10;

    public bool TryUpgrade(int i)
    {
        if (IsMaxed(i)) return false;
        int cost = GetUpgradeCost(i);
        if (GameStateManager.Instance.GetCoins() < cost) return false;
        GameStateManager.Instance.AddCoins(-cost);
        _levels[i]++;
        return true;
    }
}
