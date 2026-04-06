using System;
using UnityEngine;

public class DestroyOnGameStart : MonoBehaviour
{
    private void Start()
    {
        GameStateManager.Instance.OnStartPlaying += OnStartPlaying;
    }

    private void OnDestroy()
    {
        GameStateManager.Instance.OnStartPlaying -= OnStartPlaying;
    }

    private void OnStartPlaying(object sender, GameStateManager.GameStateChangeEventArgs e)
    {
        // Debug.Log(this.gameObject.name + "'s DestroyOnGameStart heard OnStartPlaying");
        Destroy(this.gameObject);
    }
}