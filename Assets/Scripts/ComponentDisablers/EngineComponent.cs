using System;
using UnityEngine;

public class EngineComponent : ProbeComponent
{
    [SerializeField] private ScoreIncrement myScoreIncrement;
    [SerializeField] private AudioClip engineBreakSoundEffect;
    [SerializeField] private GameObject smokeEffect;


    private void Awake()
    {
        smokeEffect.gameObject.SetActive(false);
    }

    protected override void DisableComponent()
    {
        base.DisableComponent();
        smokeEffect.gameObject.SetActive(true);
        myScoreIncrement.SetScoreMultiplier(0.5f);
        SFXController.instance.PlaySoundFXClip(engineBreakSoundEffect, this.transform, 1f);
        Debug.Log("Engine component disabled! Score increases slower");
    }

    protected override void RepairComponent()
    {
        base.RepairComponent();
        smokeEffect.gameObject.SetActive(false);
        myScoreIncrement.SetScoreMultiplier(1f);
        Debug.Log("Engine component enabled! Score increases normally");
    }
}