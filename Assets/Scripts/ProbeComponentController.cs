using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ProbeComponentController : MonoBehaviour
{
    [SerializeField] private List<ProbeComponentDisabler> myProbeComponentDisablers;
    [SerializeField] private ProbeController myProbeController;
    [SerializeField] private float chanceToDisableComponentOnDamage = 0.3f;

    private void Start()
    {
        myProbeController.OnTakeDamage += HandleProbeTakeDamage;
    }

    private void HandleProbeTakeDamage(object sender, System.EventArgs e)
    {
        if (Random.value <= chanceToDisableComponentOnDamage)
        {
            DisableRandomComponent();
        }
    }

    public void DisableRandomComponent()
    {
        //Get all candidates that are not disabled and have a positive weight
        List<ProbeComponentDisabler> candidates = myProbeComponentDisablers.
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
        foreach (ProbeComponentDisabler candidate in candidates)
        {
            cumulative += candidate.GetWeight();
            if (randomTargetWeight <= cumulative)
            {
                candidate.TryDisable();
                break;
            }
        }
    }
}
