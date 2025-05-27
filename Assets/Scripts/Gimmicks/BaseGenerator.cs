using UnityEngine;

public abstract class BaseGenerator : MonoBehaviour
{
    [SerializeField] protected float capacityBoost = 50f;
    protected bool isRunning = false;

    protected virtual void Start()
    {
        if (isRunning)
        {
            PowerManager.Instance?.AddPowerCapacity(capacityBoost);
        }
    }

    protected virtual void OnDestroy()
    {
        if (isRunning)
        {
            PowerManager.Instance?.RemovePowerCapacity(capacityBoost);
        }
    }

    protected void StartGenerator()
    {
        if (!isRunning)
        {
            isRunning = true;
            PowerManager.Instance?.AddPowerCapacity(capacityBoost);
            Debug.Log($"{gameObject.name} ���d�J�n�i+{capacityBoost}W�j");
        }
    }

    protected void StopGenerator()
    {
        if (isRunning)
        {
            isRunning = false;
            PowerManager.Instance?.RemovePowerCapacity(capacityBoost);
            Debug.Log($"{gameObject.name} ���d��~�i-{capacityBoost}W�j");
        }
    }
}
