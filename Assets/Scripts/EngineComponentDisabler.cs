using UnityEngine;

public class EngineComponentDisabler : ProbeComponentDisabler
{
    protected override void DisableComponent()
    {
        base.DisableComponent();
        Debug.Log("Engine component disabled!");
    }
}
