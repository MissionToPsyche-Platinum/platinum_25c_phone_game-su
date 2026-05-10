using UnityEngine;
using UnityEngine.UI;

public class TabManager : MonoBehaviour
{
    [Header(("Buttons"))]
    [SerializeField] private Button probeButton;
    [SerializeField] private Button componentButton;
    [SerializeField] private Button powerUpButton;

    [SerializeField] private Text probeText;
    [SerializeField] private Text componentText;
    [SerializeField] private Text powerUpText;

    [SerializeField] private Color selectedColor;
    [SerializeField] private Color inactiveColor;

    public GameObject[] tabPages;
    int currentTab;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        probeButton.onClick.AddListener(SwitchToProbe);
        componentButton.onClick.AddListener(SwitchToComponent);
        powerUpButton.onClick.AddListener(SwitchToPowerUp);
        currentTab = 0;
        SelectTab(0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SwitchToProbe()
    {
        SelectTab(0);
        probeText.color = selectedColor;
        componentText.color = inactiveColor;
        powerUpText.color = inactiveColor;
    }

    public void SwitchToComponent()
    {
        SelectTab(1);
        probeText.color = inactiveColor;
        componentText.color = selectedColor;
        powerUpText.color = inactiveColor;
    }

    public void SwitchToPowerUp()
    {
        SelectTab(2);
        probeText.color = inactiveColor;
        componentText.color = inactiveColor;
        powerUpText.color = selectedColor;
    }

    public void SelectTab(int tab)
    {
        currentTab = tab;
        for(int i = 0; i < tabPages.Length; i++)
        {
            if(i == currentTab)
            {
                tabPages[i].SetActive(true);
            }
            else
            {
                tabPages[i].SetActive(false);
            }
        }
    }
}
