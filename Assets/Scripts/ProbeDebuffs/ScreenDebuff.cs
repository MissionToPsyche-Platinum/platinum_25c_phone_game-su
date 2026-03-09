using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScreenDebuff : ProbeDebuff
{
    [SerializeField] private GameObject crackedScreenPanel;
    [SerializeField] private AudioClip crackedScreenSoundEffect;
    [SerializeField] private float fadeDuration = 2f;
    private Image crackedScreenImage;
    private Coroutine fadeCoroutine;

    private void Start()
    {
        crackedScreenImage = crackedScreenPanel.GetComponentInChildren<Image>();
        crackedScreenPanel.SetActive(false);
    }

    protected override void EnableDebuff()
    {
        base.EnableDebuff();
        Debug.Log("Enabling screen debuff");
        SFXController.instance.PlaySoundFXClip(crackedScreenSoundEffect, crackedScreenPanel.transform, 1f);
        ShowCrackedScreen();
    }

    protected override void RepairDebuff()
    {
        base.RepairDebuff();
        HideCrackedScreen();
    }
    
    

    private void ShowCrackedScreen()
    {
        crackedScreenPanel.SetActive(true);
        SetCrackedScreenAlphaToFull();
        FadeCrackedScreen();
    }
    
    private void HideCrackedScreen()
    {
        crackedScreenPanel.SetActive(false);
    }

    private void SetCrackedScreenAlphaToFull()
    {
        Color c = crackedScreenImage.color;
        c.a = 1f;
        crackedScreenImage.color = c;
    }
    
    private void FadeCrackedScreen()
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadeOutRoutine());
    }

    //slowly fades out the 
    private IEnumerator FadeOutRoutine()
    {
        float elapsed = 0f;
        Color crackedScreenImageColor = crackedScreenImage.color;

        while (elapsed < fadeDuration)
        {
            //Slowly fade the alpha to 0
            elapsed += Time.deltaTime;
            crackedScreenImageColor.a = 1f - Mathf.Clamp01(elapsed / fadeDuration);
            crackedScreenImage.color = crackedScreenImageColor;
            yield return null;
        }

        crackedScreenImageColor.a = 0f;
        crackedScreenImage.color = crackedScreenImageColor;
        HideCrackedScreen();
        fadeCoroutine = null;
        RepairDebuff();
    }
}