using UnityEngine;

public class ComponentManager : MonoBehaviour
{
    public static ComponentManager Instance { get; private set; }

    public static readonly string[] Names =
    {
        "Multispectral Imager",
        "Gamma Ray & Neutron Detector",
        "Magnetometer",
        "Ion Propulsion System",
        "Radio Science Subsystem",
        "Solar Panel Array",
        "Navigation Camera",
        "X-band Antenna"
    };

    public static readonly string[] Descriptions =
    {
        "Slows asteroid speed by 10%",
        "Coins spawn 2x faster",
        "Score increases 1.25x faster",
        "Lateral movement 50% faster",
        "25% less debris spawned",
        "Regenerate 1 HP every 15 seconds",
        "25% more vision",
        "Score and coins earn 10% faster"
    };

    public static readonly int[] Costs = { 5, 15, 25, 35, 10, 20, 30, 40 };

    private bool[] _unlocked     = new bool[8];
    private int    _equippedLeft  = -1;
    private int    _equippedRight = -1;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public bool IsUnlocked(int i) => _unlocked[i];
    public int  GetEquippedLeft()  => _equippedLeft;
    public int  GetEquippedRight() => _equippedRight;
    public bool IsEquipped(int i)  => i == _equippedLeft || i == _equippedRight;

    public bool TryUnlock(int i)
    {
        if (_unlocked[i] || GameStateManager.Instance.GetCoins() < Costs[i]) return false;
        GameStateManager.Instance.AddCoins(-Costs[i]);
        _unlocked[i] = true;
        Save();
        return true;
    }

    public void EquipLeft(int i)
    {
        if (i < 0 || i > 3 || !_unlocked[i]) return;
        _equippedLeft = (_equippedLeft == i) ? -1 : i;
        Save();
    }

    public void EquipRight(int i)
    {
        if (i < 4 || i > 7 || !_unlocked[i]) return;
        _equippedRight = (_equippedRight == i) ? -1 : i;
        Save();
    }

    public float GetDebrisSpeedMultiplier() => _equippedLeft == 0 ? 0.9f : 1f;

    public float GetCoinSpawnMultiplier()
    {
        float m = 1f;
        if (_equippedLeft  == 1) m *= 2f;
        if (_equippedRight == 7) m *= 1.1f;
        return m;
    }

    public float GetScoreMultiplier()
    {
        float m = 1f;
        if (_equippedLeft  == 2) m *= 1.25f;
        if (_equippedRight == 7) m *= 1.1f;
        return m;
    }

    public float GetLateralSpeedMultiplier() => _equippedLeft == 3 ? 1.5f : 1f;

    public float GetDebrisSpawnIntervalMultiplier() => _equippedRight == 4 ? (1f / 0.75f) : 1f;

    public bool  HasHPRegen()         => _equippedRight == 5;
    public float GetHPRegenInterval() => 15f;

    public float GetVisionMultiplier() => _equippedRight == 6 ? 1.25f : 1f;

    [ContextMenu("Reset Component Data")]
    public void ResetAllData()
    {
        for (int i = 0; i < 8; i++)
            PlayerPrefs.DeleteKey("Comp_Unlocked_" + i);
        PlayerPrefs.DeleteKey("Comp_Left");
        PlayerPrefs.DeleteKey("Comp_Right");
        PlayerPrefs.Save();

        _unlocked      = new bool[8];
        _equippedLeft  = -1;
        _equippedRight = -1;
    }

    private void Save()
    {
        for (int i = 0; i < 8; i++)
            PlayerPrefs.SetInt("Comp_Unlocked_" + i, _unlocked[i] ? 1 : 0);
        PlayerPrefs.SetInt("Comp_Left",  _equippedLeft);
        PlayerPrefs.SetInt("Comp_Right", _equippedRight);
        PlayerPrefs.Save();
    }

    private void Load()
    {
        for (int i = 0; i < 8; i++)
            _unlocked[i] = PlayerPrefs.GetInt("Comp_Unlocked_" + i, 0) == 1;
        _equippedLeft  = PlayerPrefs.GetInt("Comp_Left",  -1);
        _equippedRight = PlayerPrefs.GetInt("Comp_Right", -1);
        if (_equippedLeft  != -1 && !_unlocked[_equippedLeft])  _equippedLeft  = -1;
        if (_equippedRight != -1 && !_unlocked[_equippedRight]) _equippedRight = -1;
    }
}
