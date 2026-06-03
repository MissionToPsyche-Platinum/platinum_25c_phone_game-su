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
    private int[,] upgradeCosts = {
//level  0   1   2   3   4
        {5, 10, 20, 40, 100},   //hyperspace
        {5, 10, 20, 40, 100},   //force field
        {5, 10, 20, 40, 100},   //R.U.M.
        {5, 10, 20, 40, 100},   //shockwave
        {5, 10, 20, 40, 100}    //laser drill
    };
    private int[,] durations = {
//level  0   1   2   3   4   5
        {5, 10, 15, 20, 25, 30},    //hyperspace
        {5, 10, 15, 20, 25, 30},    //force field
        {5, 10, 15, 20, 25, 30},    //R.U.M.
        {1, 2, 3, 4, 5, 6},           //shockwave
        {5, 10, 15, 20, 25, 30}     //laser drill
    };

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public int GetLevel(int i) => _levels[i];
    public bool IsMaxed(int i) => _levels[i] >= MaxLevel;
    public int GetUpgradeCost(int i) => i <= 4 ? upgradeCosts[i, _levels[i]] : 99999999;
    public float GetDuration(int i) => i <= 5 ? durations[i, _levels[i]] : 0;

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
