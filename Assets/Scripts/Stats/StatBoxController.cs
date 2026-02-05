using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class StatBoxController : MonoBehaviour
{
    [SerializeField] private protected Sprite checkmarkImage;
    [SerializeField] private Sprite xImage;
    
    [SerializeField] private protected TextMeshProUGUI displayedText;
    [SerializeField] private Image checkOrX;

    private void OnEnable()
    {
        SetAppropriateVisual();
    }

    protected abstract void SetAppropriateVisual();

    public void ShowCheckmark()
    {
        checkOrX.gameObject.SetActive(true);
        checkOrX.sprite = checkmarkImage;
    }

    public void ShowX()
    {
        checkOrX.gameObject.SetActive(true);
        checkOrX.sprite = xImage;
    }

    public void ShowNothingOnImage()
    {
        checkOrX.gameObject.SetActive(false);
    }
}