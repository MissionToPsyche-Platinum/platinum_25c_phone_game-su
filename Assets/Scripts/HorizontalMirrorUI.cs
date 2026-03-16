using UnityEngine;

/// <summary>
/// Attach to any RectTransform to mirror it horizontally when left-handed mode is on.
/// Mirrors by reflecting the anchored position and anchors across the centre of the parent,
/// so the element moves to the opposite side of the screen.
/// </summary>
[RequireComponent(typeof(RectTransform))]
public class HorizontalMirrorUI : MonoBehaviour
{
    private RectTransform _rt;
    private Vector2 _originalAnchorMin;
    private Vector2 _originalAnchorMax;
    private Vector2 _originalAnchoredPosition;

    private void Awake()
    {
        _rt = GetComponent<RectTransform>();
        _originalAnchorMin = _rt.anchorMin;
        _originalAnchorMax = _rt.anchorMax;
        _originalAnchoredPosition = _rt.anchoredPosition;
    }

    private void OnEnable()
    {
        LeftHandedManager.OnLeftHandedChanged += ApplyMirror;
        ApplyMirror(LeftHandedManager.IsLeftHanded);
    }

    private void OnDisable()
    {
        LeftHandedManager.OnLeftHandedChanged -= ApplyMirror;
    }

    private void ApplyMirror(bool isLeftHanded)
    {
        if (isLeftHanded)
        {
            _rt.anchorMin = new Vector2(1f - _originalAnchorMax.x, _originalAnchorMin.y);
            _rt.anchorMax = new Vector2(1f - _originalAnchorMin.x, _originalAnchorMax.y);
            _rt.anchoredPosition = new Vector2(-_originalAnchoredPosition.x, _originalAnchoredPosition.y);
        }
        else
        {
            _rt.anchorMin = _originalAnchorMin;
            _rt.anchorMax = _originalAnchorMax;
            _rt.anchoredPosition = _originalAnchoredPosition;
        }
    }
}
