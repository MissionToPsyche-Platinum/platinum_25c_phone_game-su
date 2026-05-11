using UnityEngine;

public class PowerUpUpgradeManager : MonoBehaviour
{
    public static PowerUpUpgradeManager Instance { get; private set; }

    public static readonly string[] Names =
    {
        "Hyperspace", "Force Field", "R.U.M.", "Shockwave", "Laser Drill"
    };

    public const int MaxLevel = 5;

    private int[] _levels = new int[5];

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public int GetLevel(int i) => _levels[i];
    public bool IsMaxed(int i) => _levels[i] >= MaxLevel;
    public int GetUpgradeCost(int i) => 3 + (_levels[i] * 3);
    public float GetDuration(int i) => (_levels[i] + 1f) * 5f;

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
