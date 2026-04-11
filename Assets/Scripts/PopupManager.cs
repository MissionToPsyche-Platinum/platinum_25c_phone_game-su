using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PopupManager : MonoBehaviour
{
    public static PopupManager Instance { get; private set; }

    [SerializeField] private RectTransform gameScreenPanel;
    [SerializeField] private float slideDistance = 150f;
    [SerializeField] private float slideDuration = 0.4f;

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