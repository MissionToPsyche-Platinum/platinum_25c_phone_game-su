using UnityEngine;
using UnityEngine.UI;

public class TabManager : MonoBehaviour
{
    [Header(("Buttons"))]
    [SerializeField] private Button[] buttons;
    [SerializeField] private Text[] buttonTexts;

    [Header(("Colors"))]
    [SerializeField] private Color selectedColor;
    [SerializeField] private Color inactiveColor;

    public GameObject[] tabPages;
    int currentTab;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for(int i = 0; i < buttons.Length; i++){
            int index = i;
            buttons[i].onClick.AddListener(() => switchToTab(index));
        }
        currentTab = 0;
        SelectTab(0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void switchToTab(int index){
        SelectTab(index);
        for(int i = 0; i < buttons.Length; i++){
            if(i == index){
                buttonTexts[i].color = selectedColor;
            }
            else {
                buttonTexts[i].color = inactiveColor;
            }
        }
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
