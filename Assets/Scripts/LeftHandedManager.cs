using System;
using UnityEngine;

public class LeftHandedManager : MonoBehaviour
{
    public static LeftHandedManager Instance { get; private set; }

    public static event Action<bool> OnLeftHandedChanged;

    public static bool IsLeftHanded { get; private set; }

    private const string PrefKey = "left_handed_mode";

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
        bool saved = PlayerPrefs.GetInt(PrefKey, 0) == 1;
        ApplyMode(saved, notify: false);
    }

    public void SetLeftHanded(bool enabled)
    {
        ApplyMode(enabled, notify: true);
        PlayerPrefs.SetInt(PrefKey, enabled ? 1 : 0);
    }

    private void ApplyMode(bool enabled, bool notify)
    {
        IsLeftHanded = enabled;
        if (notify)
            OnLeftHandedChanged?.Invoke(enabled);
    }
}
