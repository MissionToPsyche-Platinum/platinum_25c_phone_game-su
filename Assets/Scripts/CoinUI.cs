using UnityEngine;
using UnityEngine.UI;

public class CoinUI : MonoBehaviour
{
    public Text coinText;

    private RectTransform _rt;
    private Vector2 _originalAnchoredPosition;
    private bool _mirrorEnabled;

    private void Awake()
    {
        _rt = GetComponent<RectTransform>();
        _originalAnchoredPosition = _rt.anchoredPosition;
        _mirrorEnabled = transform.parent != null && transform.parent.GetComponent<HorizontalMirrorUI>() != null;
    }

    private void OnEnable()
    {
        if (!_mirrorEnabled) return;
        LeftHandedManager.OnLeftHandedChanged += ApplyMirror;
        ApplyMirror(LeftHandedManager.IsLeftHanded);
    }

    private void OnDisable()
    {
        if (!_mirrorEnabled) return;
        LeftHandedManager.OnLeftHandedChanged -= ApplyMirror;
    }

    private void ApplyMirror(bool isLeftHanded)
    {
        Vector2 pos = _originalAnchoredPosition;
        pos.x = isLeftHanded ? -Mathf.Abs(pos.x) : Mathf.Abs(pos.x);
        _rt.anchoredPosition = pos;
    }

    void Update()
    {
        if (coinText != null && GameStateManager.Instance != null)
            coinText.text = GameStateManager.Instance.GetCoins().ToString();
    }
}
