using UnityEngine;
using UnityEngine.UI;

public class LeftHandedSettingsUI : MonoBehaviour
{
    [SerializeField] private Toggle leftHandedToggle;

    // Prevents callbacks from firing while we programmatically set toggle state
    private bool _isInitializing;

    private void Start()
    {
        leftHandedToggle.onValueChanged.AddListener(isOn =>
        {
            if (!_isInitializing)
                Apply(isOn);
        });
    }

    private void OnEnable()
    {
        RefreshToggle();
    }

    private void RefreshToggle()
    {
        _isInitializing = true;
        leftHandedToggle.isOn = LeftHandedManager.IsLeftHanded;
        _isInitializing = false;
    }

    private void Apply(bool isLeftHanded)
    {
        if (LeftHandedManager.Instance == null)
        {
            Debug.LogError("[LeftHandedSettingsUI] LeftHandedManager.Instance is null — add LeftHandedManager component to a scene GameObject.");
            return;
        }
        LeftHandedManager.Instance.SetLeftHanded(isLeftHanded);
    }
}
