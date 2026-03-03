using UnityEngine;
using UnityEngine.UI;

public class DisclaimerPanelController : MonoBehaviour
{
    [SerializeField] private Button continueButton;
    [SerializeField] private GameObject startScreenPanel;

    private void Awake()
    {
        continueButton.onClick.AddListener(Dismiss);
    }

    private void Dismiss()
    {
        MenuNavigationButtons.Instance.PlayButtonSound();
        gameObject.SetActive(false);
        startScreenPanel.SetActive(true);
    }

    public void Show()
    {
        MenuNavigationButtons.Instance.PlayButtonSound();
        gameObject.SetActive(true);
    }
}
