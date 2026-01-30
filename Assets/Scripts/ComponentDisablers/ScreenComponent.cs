using System;
using UnityEngine;

public class ScreenComponent : ProbeComponent
{
    [SerializeField] private GameObject crackedScreenPanel;
    [SerializeField] private AudioClip crackedScreenSoundEffect;

    private void Start()
    {
        crackedScreenPanel.SetActive(false);
    }

    protected override void DisableComponent()
    {
        base.DisableComponent();
        Debug.Log("Disabling screen component");
        SFXController.instance.PlaySoundFXClip(crackedScreenSoundEffect, crackedScreenPanel.transform, 1f);
        ShowCrackedScreen();
    }

    protected override void RepairComponent()
    {
        base.RepairComponent();
        HideCrackedScreen();
    }
    
    

    private void ShowCrackedScreen()
    {
        crackedScreenPanel.SetActive(true);
    }
    
    private void HideCrackedScreen()
    {
        crackedScreenPanel.SetActive(false);
    }
}