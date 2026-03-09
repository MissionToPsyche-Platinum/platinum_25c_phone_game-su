using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Random = UnityEngine.Random;

public class ProbeDebuffController : MonoBehaviour
{
    [SerializeField] private List<ProbeDebuff> myProbeDebuffs;
    [SerializeField] private ProbeController myProbeController;
    [SerializeField] private float chanceToDebuffOnDamage = 0.3f;

    private void Start()
    {
        myProbeController.OnTakeDamage += HandleProbeTakeDamage;
        GameStateManager.Instance.OnStartPlaying += OnStartPlaying;
        GameStateManager.Instance.OnStopPlaying += OnStopPlaying;
    }

    private void OnDestroy()
    {
        myProbeController.OnTakeDamage -= HandleProbeTakeDamage;
        GameStateManager.Instance.OnStartPlaying -= OnStartPlaying;
        GameStateManager.Instance.OnStopPlaying -= OnStopPlaying;
    }

    private void HandleProbeTakeDamage(object sender, System.EventArgs e)
    {
        if (Random.value <= chanceToDebuffOnDamage)
        {
            EnableRandomDebuff();
        }
    }

    public void EnableRandomDebuff()
    {
        //Get all candidates that are not disabled and have a positive weight
        List<ProbeDebuff> candidates = myProbeDebuffs.
                Where(d => !d.GetIsDisabled() && d.GetWeight() > 0f).ToList();

        if (candidates.Count == 0)
        {
            return;
        }

        float totalWeight = 0f;
        foreach (var candidate in candidates)
            totalWeight += candidate.GetWeight();

        float randomTargetWeight = Random.Range(0f, totalWeight);  // inclusive min, exclusive max[web:23]
        float cumulative = 0f;

        //Randomly select a candidate based on weight
        foreach (ProbeDebuff candidate in candidates)
        {
            cumulative += candidate.GetWeight();
            if (randomTargetWeight <= cumulative)
            {
                candidate.TryDisable();
                break;
            }
        }
    }

    private void OnStopPlaying(object sender, GameStateManager.GameStateChangeEventArgs e)
    {
        RepairAllDebuffs();
    }
    private void OnStartPlaying(object sender, GameStateManager.GameStateChangeEventArgs e)
    {
        RepairAllDebuffs();
    }

    public void RepairAllDebuffs()
    {
        foreach (ProbeDebuff debuff in myProbeDebuffs)
        {
            debuff.TryRepair();
        }
    }
}