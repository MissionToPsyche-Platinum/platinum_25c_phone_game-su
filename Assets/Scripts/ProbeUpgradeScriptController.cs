using UnityEngine;
using UnityEngine.UI;

public class ProbeUpgradeScriptController : MonoBehaviour
{
    [Header("Cards - assign indices 0 through 4 in order")]
    [SerializeField] private ProbeCardUI[] cards;

    private bool _initialized = false;

    private void Start()
    {
        for (int i = 0; i < cards.Length; i++)
            cards[i].Setup(i);
        _initialized = true;
    }

    private void OnEnable()
    {
        if (!_initialized) return; // skip the first OnEnable before Start
        RefreshAll();
    }

    public void RefreshAll()
    {
        foreach (var card in cards)
            card.Refresh();
    }
}
