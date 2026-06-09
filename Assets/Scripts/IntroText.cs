using UnityEngine;
using UnityEngine.InputSystem;

public class IntroText : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Animator startScreenAnimator;
    [SerializeField] private GameObject introPanel;

    [SerializeField] private RectTransform scrollingText;
    [SerializeField] private RectTransform topMarker;
    [SerializeField] private RectTransform bottomMarker;
    [SerializeField] private Canvas parentCanvas;

    [Header("Scroll Speeds")]
    [SerializeField] private float normalSpeed = 80f;
    [SerializeField] private float holdSpeed = 300f;
    [SerializeField] private float skipSpeed = 3000f;

    private bool skippedClicked = false;
    private bool initialized = false;
    private bool finished = false;

    private float currentSpeed;

    private void Awake()
    {
        currentSpeed = normalSpeed;
    }

    private void Start()
    {
        InitializeStartPosition();
    }

    private void FixedUpdate()
    {
        if (finished)
            return;

        if (!initialized)
            InitializeStartPosition();

        if (skippedClicked)
        {
            currentSpeed = skipSpeed;
        }
        else
        {
            bool isHolding =
                (Mouse.current != null && Mouse.current.leftButton.isPressed) ||
                (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed);

            currentSpeed = isHolding ? holdSpeed : normalSpeed;
        }

        ScrollUpward();
        CheckForEnd();
    }

    private void InitializeStartPosition()
    {
        if (scrollingText == null || topMarker == null || bottomMarker == null || parentCanvas == null)
        {
            Debug.LogError("Gotta assign stuff to IntroText.cs");
            return;
        }

        Vector2 topMarkerScreenPos = RectTransformUtility.WorldToScreenPoint(GetCanvasCamera(), topMarker.position);

        float deltaToBottomOfScreen = 0f - topMarkerScreenPos.y;
        float anchoredDelta = deltaToBottomOfScreen / parentCanvas.scaleFactor;

        Vector2 anchoredPos = scrollingText.anchoredPosition;
        anchoredPos.y += anchoredDelta;
        scrollingText.anchoredPosition = anchoredPos;
        initialized = true;
    }

    private void ScrollUpward()
    {
        Vector2 anchoredPos = scrollingText.anchoredPosition;
        anchoredPos.y += (currentSpeed * Time.deltaTime);
        scrollingText.anchoredPosition = anchoredPos;
    }

    private void CheckForEnd()
    {
        Vector2 bottomMarkerScreenPos = RectTransformUtility.WorldToScreenPoint(GetCanvasCamera(), bottomMarker.position);

        if (bottomMarkerScreenPos.y >= Screen.height)
        {
            EndScroll();
        }
    }

    private Camera GetCanvasCamera()
    {
        if (parentCanvas.renderMode == RenderMode.ScreenSpaceOverlay)
            return null;

        return parentCanvas.worldCamera;
    }

    public void EndScroll()
    {
        if (finished)
            return;

        finished = true;
        introPanel.SetActive(false);
        startScreenAnimator.SetTrigger("QueueSlide");

        StartScreenAnimationHandler ssAH = startScreenAnimator.GetComponent<StartScreenAnimationHandler>();
        ssAH.shouldSkipAnimation = true;
    }

    public void SkipAnimation()
    {
        skippedClicked = true;
    }
}
