using UnityEngine;
using UnityEngine.UI;

public class ColorBlindSettingsUI : MonoBehaviour
{
    [SerializeField] private Toggle noneToggle;
    [SerializeField] private Toggle protanopiaToggle;
    [SerializeField] private Toggle deuteranopiaToggle;
    [SerializeField] private Toggle tritanopiaToggle;

    // Prevents callbacks from firing while we programmatically set toggle states
    private bool _isInitializing;

    private void Start()
    {
        noneToggle.onValueChanged.AddListener(
            isOn => { if (isOn && !_isInitializing) Apply(ColorBlindManager.ColorBlindMode.None); });
        protanopiaToggle.onValueChanged.AddListener(
            isOn => { if (isOn && !_isInitializing) Apply(ColorBlindManager.ColorBlindMode.Protanopia); });
        deuteranopiaToggle.onValueChanged.AddListener(
            isOn => { if (isOn && !_isInitializing) Apply(ColorBlindManager.ColorBlindMode.Deuteranopia); });
        tritanopiaToggle.onValueChanged.AddListener(
            isOn => { if (isOn && !_isInitializing) Apply(ColorBlindManager.ColorBlindMode.Tritanopia); });
    }

    private void OnEnable()
    {
        RefreshToggles();
    }

    private void RefreshToggles()
    {
        ColorBlindManager.ColorBlindMode current =
            ColorBlindManager.Instance != null
                ? ColorBlindManager.Instance.GetMode()
                : ColorBlindManager.ColorBlindMode.None;

        _isInitializing = true;
        noneToggle.isOn         = current == ColorBlindManager.ColorBlindMode.None;
        protanopiaToggle.isOn   = current == ColorBlindManager.ColorBlindMode.Protanopia;
        deuteranopiaToggle.isOn = current == ColorBlindManager.ColorBlindMode.Deuteranopia;
        tritanopiaToggle.isOn   = current == ColorBlindManager.ColorBlindMode.Tritanopia;
        _isInitializing = false;
    }

    private void Apply(ColorBlindManager.ColorBlindMode mode)
    {
        if (ColorBlindManager.Instance == null)
        {
            Debug.LogError("[ColorBlindSettingsUI] ColorBlindManager.Instance is null — add ColorBlindManager component to a scene GameObject.");
            return;
        }
        Debug.Log($"[ColorBlindSettingsUI] Applying mode: {mode}");
        ColorBlindManager.Instance.SetMode(mode);
    }
}
