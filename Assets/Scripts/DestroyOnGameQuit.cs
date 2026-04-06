using UnityEngine;

public class DestroyOnGameQuit : MonoBehaviour
{
    private void Start()
    {
        GameStateManager.Instance.OnStopPlaying += OnStopPlaying;
    }

    private void OnDestroy()
    {
        GameStateManager.Instance.OnStopPlaying -=  OnStopPlaying;
    }

    private void OnStopPlaying(object sender, GameStateManager.GameStateChangeEventArgs e)
    {
        Destroy(this.gameObject);
    }
    
}