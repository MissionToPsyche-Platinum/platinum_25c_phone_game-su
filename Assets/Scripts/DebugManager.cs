using System;
using UnityEngine;

public class DebugManager : MonoBehaviour
{
    public static DebugManager Instance { get; private set; }

    public bool IsDebugUnlocked { get; private set; } = false;
    public bool GodModeEnabled  { get; private set; } = false;

    public event Action OnDebugUnlocked;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void UnlockDebugMode()
    {
        if (IsDebugUnlocked) return;
        IsDebugUnlocked = true;
        OnDebugUnlocked?.Invoke();
    }

    public void SetGodMode(bool enabled)
    {
        GodModeEnabled = enabled;
    }
}
