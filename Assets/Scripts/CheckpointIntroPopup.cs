using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CanvasGroup))]
public class CheckpointIntroPopup : MonoBehaviour
{
    [SerializeField] private float fadeDuration = 0.3f;
    [SerializeField] private float minDisplayTime = 0.5f;

    public event Action OnDismissed;

    private CanvasGroup canvasGroup;
    private bool dismissing = false;
    private float spawnTime;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;
        spawnTime = Time.unscaledTime;
    }

    private void Update()
    {
        if (dismissing) return;
        if (Time.unscaledTime - spawnTime < minDisplayTime) return;

        bool tapped = false;
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
        {
            tapped = true;
        }
        if (!tapped && Mouse.current.leftButton.wasPressedThisFrame)
        {
            tapped = true;
        }
        if (tapped) Dismiss();
    }

    public void Dismiss()
    {
        // Debug.Log("Dismiss called");
        if (dismissing) return;
        dismissing = true;
        StartCoroutine(FadeOutAndDestroy());
    }

    private IEnumerator FadeOutAndDestroy()
    {
        float elapsed = 0f;
        float startAlpha = canvasGroup.alpha;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;
        OnDismissed?.Invoke();
        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / fadeDuration);
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, t);
            yield return null;
        }
        Destroy(gameObject);
    }
}