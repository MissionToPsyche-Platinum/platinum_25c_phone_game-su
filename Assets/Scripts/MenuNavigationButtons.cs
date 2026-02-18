using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class MenuNavigationButtons : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioClip buttonSoundClip;

    public static MenuNavigationButtons Instance { get; private set; }
    

    private void Awake()
    {
        if (Instance == null)
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this.gameObject);
                return;
            }
            Instance = this;
        }
    }

    public void PlayButtonSound()
    {
        if (SFXController.instance != null && buttonSoundClip != null)
        {
            SFXController.instance.PlaySoundFXClip(buttonSoundClip, transform, 1f);
        }
    }

    public void SwitchPanels(GameObject srcPanel, GameObject dstPanel)
    {
        if(srcPanel != null) srcPanel.SetActive(false);
        if(dstPanel != null) dstPanel.SetActive(true);
    }
}