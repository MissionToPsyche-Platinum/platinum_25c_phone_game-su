using System;
using UnityEngine;

public class ThrustersComponent : ProbeComponent
{
    [SerializeField] private ProbeController myProbeController;
    [SerializeField] private GameObject thrusterSmokeEffect;
    [SerializeField] private AudioClip thrusterBreakingSoundEffect;
    
    [SerializeField] private float brokenMovementSpeed = 3f;

    private void Awake()
    {
        thrusterSmokeEffect.SetActive(false);
    }

    protected override void DisableComponent()
    {
        base.DisableComponent();
        thrusterSmokeEffect.SetActive(true);
        SFXController.instance.PlaySoundFXClip(thrusterBreakingSoundEffect, this.transform, 1f);
        myProbeController.SetMoveSpeed(brokenMovementSpeed);
    }

    protected override void RepairComponent()
    {
        base.RepairComponent();
        thrusterSmokeEffect.SetActive(false);
        myProbeController.SetMoveSpeed(4f);
    }
}