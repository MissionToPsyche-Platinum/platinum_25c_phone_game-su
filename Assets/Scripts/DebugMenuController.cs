using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DebugMenuController : MonoBehaviour
{
    [SerializeField] private Toggle godModeToggle;
    [SerializeField] private Button saveButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private TMP_Text statusText;

    private void Awake()
    {
        if (godModeToggle != null)
            godModeToggle.onValueChanged.AddListener(OnGodModeToggled);

        if (saveButton != null)
            saveButton.onClick.AddListener(OnSaveClicked);

        if (closeButton != null)
            closeButton.onClick.AddListener(Hide);
    }

    private void OnEnable()
    {
        if (DebugManager.Instance == null) return;
        if (godModeToggle != null)
            godModeToggle.SetIsOnWithoutNotify(DebugManager.Instance.GodModeEnabled);
        SetStatus(string.Empty);
    }

    private void OnGodModeToggled(bool value)
    {
        MenuNavigationButtons.Instance?.PlayButtonSound();
        DebugManager.Instance?.SetGodMode(value);
        SetStatus(value ? "God Mode: ON" : "God Mode: OFF");
    }

    private void OnSaveClicked()
    {
        MenuNavigationButtons.Instance?.PlayButtonSound();
        PlayerPrefs.Save();
        SetStatus("Progress saved.");
    }

    public void Show()
    {
        MenuNavigationButtons.Instance?.PlayButtonSound();
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        MenuNavigationButtons.Instance?.PlayButtonSound();
        gameObject.SetActive(false);
    }

    private void SetStatus(string msg)
    {
        if (statusText != null)
            statusText.SetText(msg);
    }
}
