using System;
using System.Collections;
using UnityEngine;


public class ComponentEffectApplier : MonoBehaviour
{
    [SerializeField] private CoinSpawner       coinSpawner;
    [SerializeField] private DebrisSpawnScript debrisSpawnScript;
    [SerializeField] private ProbeHealth       probeHealth;

    private Coroutine _hpRegenRoutine;

    private void Start()
    {
        GameStateManager.Instance.OnStartPlaying += OnStartPlaying;
        GameStateManager.Instance.OnStopPlaying  += OnStopPlaying;
    }

    private void OnDestroy()
    {
        GameStateManager.Instance.OnStartPlaying -= OnStartPlaying;
        GameStateManager.Instance.OnStopPlaying  -= OnStopPlaying;
    }

    private void OnStartPlaying(object sender, GameStateManager.GameStateChangeEventArgs e)
    {
        StopHPRegen();

        var cm = ComponentManager.Instance;
        coinSpawner.SetComponentSpawnMultiplier(cm.GetCoinSpawnMultiplier());
        debrisSpawnScript.SetSpawnIntervalMultiplier(cm.GetDebrisSpawnIntervalMultiplier());

        if (cm.HasHPRegen())
            _hpRegenRoutine = StartCoroutine(HPRegenLoop(cm.GetHPRegenInterval()));
    }

    private void OnStopPlaying(object sender, GameStateManager.GameStateChangeEventArgs e)
    {
        StopHPRegen();
    }

    private IEnumerator HPRegenLoop(float interval)
    {
        while (true)
        {
            yield return new WaitForSeconds(interval);
            if (GameStateManager.Instance.currentGameState == GameStateManager.GameState.Playing)
                probeHealth.RegenHP(1);
        }
    }

    private void StopHPRegen()
    {
        if (_hpRegenRoutine == null) return;
        StopCoroutine(_hpRegenRoutine);
        _hpRegenRoutine = null;
    }
}
