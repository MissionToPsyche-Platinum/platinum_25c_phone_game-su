using UnityEngine;

public class TipManager : MonoBehaviour
{
    [SerializeField] private GameObject[] tipPopupPrefabs; // assign 3 entries in Inspector

    private const string PlayCountKey = "PlayCount";
    private const int MaxTipPlays = 3;

    private void Start()
    {
        if (GameStateManager.Instance != null)
            GameStateManager.Instance.OnStartPlaying += OnStartPlaying;
    }

    private void OnDestroy()
    {
        if (GameStateManager.Instance != null)
            GameStateManager.Instance.OnStartPlaying -= OnStartPlaying;
    }

    private void OnStartPlaying(object sender, GameStateManager.GameStateChangeEventArgs e)
    {
        int playCount = PlayerPrefs.GetInt(PlayCountKey, 0);
        playCount++;
        PlayerPrefs.SetInt(PlayCountKey, playCount);
        PlayerPrefs.Save();

        int tipIndex = playCount - 1;
        if (tipIndex < MaxTipPlays && tipPopupPrefabs != null && tipIndex < tipPopupPrefabs.Length
            && tipPopupPrefabs[tipIndex] != null)
        {
            PopupManager.Instance.DisplayPopup(tipPopupPrefabs[tipIndex], 6f);
        }
    }
}
