using UnityEngine;

public class PowerUpUpgradeManager : MonoBehaviour
{
    public static PowerUpUpgradeManager Instance { get; private set; }

    public static readonly string[] Names =
    {
        "Hyperspace", "Shield", "2x Coins", "Star", "Hex"
    };

    public const int MaxLevel = 10;

    private int[] _levels = new int[5];

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public int GetLevel(int i) => _levels[i];
    public bool IsMaxed(int i) => _levels[i] >= MaxLevel;
    public int GetUpgradeCost(int i) => 10 * (_levels[i] + 1);
    public float GetSpawnMultiplier(int i) => 1f + (_levels[i] * 0.1f);
    public float GetDurationMultiplier(int i) => 1f + (_levels[i] * 0.1f);

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
