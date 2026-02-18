using UnityEngine;

//Base class for all probe components
public class ProbeComponent : MonoBehaviour
{
    private bool isDisabled = false;
    [SerializeField] private float weight;
    protected virtual void DisableComponent()
    {
        isDisabled = true;
    }

    protected virtual void RepairComponent()
    {
        isDisabled = false;
    }

    public void TryDisable()
    {
        if (!isDisabled)
        {
            DisableComponent();
        }
        else
        {
            Debug.LogError("Tried to disable a component that is already disabled.");
        }
    }
    
    public void TryRepair()
    {
        if (isDisabled)
        {
            RepairComponent();
        }
        else
        {
            Debug.Log("Tried to repair a component that is already repaired. Skipping.");
        }
    }

    public bool GetIsDisabled()
    {
        return isDisabled;
    }

    public float GetWeight()
    {
        return weight;
    }
}