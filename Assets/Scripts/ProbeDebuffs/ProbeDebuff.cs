using UnityEngine;

//Base class for all probe components
public class ProbeDebuff : MonoBehaviour
{
    private bool isDisabled = false;
    [SerializeField] private float weight;
    protected virtual void EnableDebuff()
    {
        isDisabled = true;
    }

    protected virtual void RepairDebuff()
    {
        isDisabled = false;
    }

    public void TryDisable()
    {
        if (!isDisabled)
        {
            EnableDebuff();
        }
        else
        {
            Debug.LogError("Tried to enable a debuff that is already enabled.");
        }
    }
    
    public void TryRepair()
    {
        if (isDisabled)
        {
            RepairDebuff();
        }
        else
        {
            // Debug.Log("Tried to repair a debuff that is already repaired. Skipping.");
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