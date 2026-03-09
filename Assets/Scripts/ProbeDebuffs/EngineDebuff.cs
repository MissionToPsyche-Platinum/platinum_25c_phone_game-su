using System;
using UnityEngine;

public class EngineDebuff : ProbeDebuff
{
    [SerializeField] private ScoreIncrement myScoreIncrement;
    [SerializeField] private AudioClip engineBreakSoundEffect;
    [SerializeField] private GameObject smokeEffect;


    private void Awake()
    {
        smokeEffect.gameObject.SetActive(false);
    }

    protected override void EnableDebuff()
    {
        base.EnableDebuff();
        smokeEffect.gameObject.SetActive(true);
        myScoreIncrement.SetScoreMultiplier(0.5f);
        SFXController.instance.PlaySoundFXClip(engineBreakSoundEffect, this.transform, 1f);
        Debug.Log("Engine debuff enabled! Score increases slower");
    }

    protected override void RepairDebuff()
    {
        base.RepairDebuff();
        smokeEffect.gameObject.SetActive(false);
        myScoreIncrement.SetScoreMultiplier(1f);
        Debug.Log("Engine debuff repaired! Score increases normally");
    }
}