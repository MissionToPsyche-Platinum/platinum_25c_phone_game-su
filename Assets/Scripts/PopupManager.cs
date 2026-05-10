using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PopupManager : MonoBehaviour
{
    public static PopupManager Instance { get; private set; }

    [SerializeField] private RectTransform gameScreenPanel;
    [SerializeField] private float slideDistance = 150f;
    [SerializeField] private float slideDuration = 0.4f;
    [SerializeField] private float fadeDuration = 0.3f;
    private const float growDuration = 1f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void DisplayPopup(GameObject popupPrefab, float displayDuration)
    {
        StartCoroutine(PopupRoutine(popupPrefab, displayDuration));
    }

    public void DisplayCenteredPopup(GameObject popupPrefab, float displayDuration)
    {
        StartCoroutine(CenteredPopupRoutine(popupPrefab, displayDuration));
    }

    public GameObject DisplayFullScreenPopup(GameObject popupPrefab)
    {
        GameObject popup = Instantiate(popupPrefab, gameScreenPanel);
        RectTransform rect = popup.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        rect.anchoredPosition = Vector2.zero;

        CanvasGroup cg = popup.GetComponent<CanvasGroup>();
        if (cg == null) cg = popup.AddComponent<CanvasGroup>();
        cg.alpha = 0f;
        popup.transform.localScale = Vector3.zero;
        StartCoroutine(GrowFromCenter(cg, popup.transform, growDuration));
        return popup;
    }

    private IEnumerator GrowFromCenter(CanvasGroup cg, Transform popupTransform, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            if (cg == null) yield break;
            elapsed += Time.unscaledDeltaTime;
            float raw = Mathf.Clamp01(elapsed / duration);
            // ease-out: overshoot then settle
            float t = raw;
            cg.alpha = t;
            if (popupTransform != null)
                popupTransform.localScale = Vector3.one * t;
            yield return null;
        }
        if (cg != null) cg.alpha = 1f;
        if (popupTransform != null) popupTransform.localScale = Vector3.one;
    }

    private float GetTopSafeAreaOffset()
    {
        float topInsetPixels = Screen.height - Screen.safeArea.yMax;
        if (topInsetPixels <= 0f) return 0f;
        Canvas canvas = gameScreenPanel.GetComponentInParent<Canvas>();
        if (canvas == null) return 0f;
        return topInsetPixels / canvas.scaleFactor;
    }

    private IEnumerator PopupRoutine(GameObject popupPrefab, float displayDuration)
    {
        // Spawn at top of panel, shifted down past the notch
        GameObject popup = Instantiate(popupPrefab, gameScreenPanel);
        RectTransform rect = popup.GetComponent<RectTransform>();

        float safeOffset = GetTopSafeAreaOffset();
        float hiddenY = -safeOffset;
        float visibleY = -(slideDistance + safeOffset);

        rect.anchoredPosition = new Vector2(0f, hiddenY);

        yield return StartCoroutine(SlideY(rect, hiddenY, visibleY, slideDuration));

        //wait and let the popup chill on screen
        yield return new WaitForSeconds(displayDuration);

        yield return StartCoroutine(SlideY(rect, visibleY, hiddenY, slideDuration));

        Destroy(popup);
    }

    private IEnumerator CenteredPopupRoutine(GameObject popupPrefab, float displayDuration)
    {
        GameObject popup = Instantiate(popupPrefab, gameScreenPanel);
        RectTransform rect = popup.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = Vector2.zero;

        CanvasGroup cg = popup.GetComponent<CanvasGroup>();
        if (cg == null) cg = popup.AddComponent<CanvasGroup>();
        cg.alpha = 0f;
        cg.blocksRaycasts = false;
        cg.interactable = false;

        yield return StartCoroutine(FadeCanvasGroup(cg, 0f, 1f, fadeDuration));
        yield return new WaitForSeconds(displayDuration);
        yield return StartCoroutine(FadeCanvasGroup(cg, 1f, 0f, fadeDuration));

        Destroy(popup);
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup cg, float from, float to, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            if (cg == null) yield break;
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);
            cg.alpha = Mathf.Lerp(from, to, t);
            yield return null;
        }
        if (cg != null) cg.alpha = to;
    }

    private IEnumerator SlideY(RectTransform rect, float from, float to, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            if (rect == null)
            {
                yield break;
            }
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);
            rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, Mathf.Lerp(from, to, t));
            yield return null;
        }
        rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, to);
    }
}
