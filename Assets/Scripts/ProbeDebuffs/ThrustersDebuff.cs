using System;
using UnityEngine;

public class ThrustersDebuff : ProbeDebuff
{
    [SerializeField] private ProbeController myProbeController;
    [SerializeField] private GameObject thrusterSmokeEffect;
    [SerializeField] private AudioClip thrusterBreakingSoundEffect;
    
    [SerializeField] private float brokenMovementSpeed = 3f;

    private void Awake()
    {
        thrusterSmokeEffect.SetActive(false);
    }

    protected override void EnableDebuff()
    {
        base.EnableDebuff();
        thrusterSmokeEffect.SetActive(true);
        SFXController.instance.PlaySoundFXClip(thrusterBreakingSoundEffect, this.transform, 1f);
        // myProbeController.SetMoveSpeed(brokenMovementSpeed);
    }

    protected override void RepairDebuff()
    {
        base.RepairDebuff();
        thrusterSmokeEffect.SetActive(false);
        // myProbeController.SetMoveSpeed(4f);?
    }
}