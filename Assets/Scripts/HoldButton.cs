using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Attach to the same GameObject as a Button to add hold-to-activate behaviour.
/// </summary>
public class HoldButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    [SerializeField] public float holdDuration = 3f;

    /// <summary>Fired once when the pointer has been held for holdDuration seconds.</summary>
    public event Action OnHoldComplete;

    // Optional fill image to show progress (assign in inspector, can be null)
    [SerializeField] private Image fillProgressImage;

    private bool _isHolding;
    private float _holdTimer;
    private bool _hasTriggered;

    private void Update()
    {
        if (!_isHolding || _hasTriggered) return;

        _holdTimer += Time.unscaledDeltaTime;

        if (fillProgressImage != null)
            fillProgressImage.fillAmount = _holdTimer / holdDuration;

        if (_holdTimer >= holdDuration)
        {
            _hasTriggered = true;
            _isHolding = false;

            if (fillProgressImage != null)
                fillProgressImage.fillAmount = 0f;

            OnHoldComplete?.Invoke();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _isHolding = true;
        _holdTimer = 0f;
        _hasTriggered = false;

        if (fillProgressImage != null)
            fillProgressImage.fillAmount = 0f;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        CancelHold();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        CancelHold();
    }

    private void CancelHold()
    {
        _isHolding = false;
        _holdTimer = 0f;

        if (fillProgressImage != null)
            fillProgressImage.fillAmount = 0f;
    }
}
