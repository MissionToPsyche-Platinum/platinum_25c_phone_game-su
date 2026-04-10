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

    private IEnumerator PopupRoutine(GameObject popupPrefab, float displayDuration)
    {
        // Spawn at top of panel
        GameObject popup = Instantiate(popupPrefab, gameScreenPanel);
        RectTransform rect = popup.GetComponent<RectTransform>();

        rect.anchoredPosition = Vector2.zero;

        yield return StartCoroutine(SlideY(rect, 0f, -slideDistance, slideDuration));

        //wait and let the popup chill on screen
        yield return new WaitForSeconds(displayDuration);

        yield return StartCoroutine(SlideY(rect, -slideDistance, 0f, slideDuration));

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