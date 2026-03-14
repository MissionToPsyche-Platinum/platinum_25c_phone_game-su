using UnityEngine;

public class ColorBlindManager : MonoBehaviour
{
    public enum ColorBlindMode { None = 0, Protanopia = 1, Deuteranopia = 2, Tritanopia = 3 }

    public static ColorBlindManager Instance { get; private set; }

    // Checked by ColorBlindRenderFeature to skip the pass when inactive
    public static bool IsActive { get; private set; }

    private static readonly int ShaderModeID = Shader.PropertyToID("_ColorBlindMode");
    // Note: SetGlobalFloat used because SetGlobalInt is unreliable in URP fragment shaders
    private const string PrefKey = "colorblind_mode";

    private ColorBlindMode _currentMode = ColorBlindMode.None;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadSavedMode();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void LoadSavedMode()
    {
        int saved = PlayerPrefs.GetInt(PrefKey, 0);
        SetMode((ColorBlindMode)saved);
    }

    public void SetMode(ColorBlindMode mode)
    {
        _currentMode = mode;
        IsActive = mode != ColorBlindMode.None;
        Shader.SetGlobalFloat(ShaderModeID, (float)mode);
        PlayerPrefs.SetInt(PrefKey, (int)mode);
        Debug.Log($"[ColorBlindManager] Mode set to {mode} (shader int = {(int)mode})");
    }

    public ColorBlindMode GetMode() => _currentMode;
}
