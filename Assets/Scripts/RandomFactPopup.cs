using TMPro;
using UnityEngine;

public class RandomFactPopup : MonoBehaviour
{
    [SerializeField] private TMP_Text bodyText;
    [TextArea(3, 6)]
    [SerializeField] private string[] facts;

    private void Awake()
    {
        if (facts.Length > 0 && bodyText != null)
            bodyText.text = facts[Random.Range(0, facts.Length)];
    }
}
