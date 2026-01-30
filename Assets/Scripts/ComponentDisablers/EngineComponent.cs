using UnityEngine;

public class EngineComponent : ProbeComponent
{
    [SerializeField] private ScoreIncrement myScoreIncrement;
    [SerializeField] private AudioClip engineBreakSoundEffect;
    
    protected override void DisableComponent()
    {
        base.DisableComponent();
        myScoreIncrement.SetScoreMultiplier(0.5f);
        SFXController.instance.PlaySoundFXClip(engineBreakSoundEffect, this.transform, 1f);
        Debug.Log("Engine component disabled! Score increases slower");
    }

    protected override void RepairComponent()
    {
        base.RepairComponent();
        myScoreIncrement.SetScoreMultiplier(1f);
        Debug.Log("Engine component enabled! Score increases normally");
    }
}