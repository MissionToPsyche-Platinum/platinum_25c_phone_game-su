using UnityEngine;
using UnityEngine.UI;

public class TabManager : MonoBehaviour
{
    [Header(("Buttons"))]
    [SerializeField] private Button probeButton;
    [SerializeField] private Button componentButton;
    [SerializeField] private Button powerUpButton;

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
    }

    public void SwitchToComponent()
    {
        SelectTab(1);
    }

    public void SwitchToPowerUp()
    {
        SelectTab(2);
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
