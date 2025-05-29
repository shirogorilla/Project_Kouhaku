using UnityEngine;

public class LightObject : ElectricAppliance
{
    [SerializeField] private GameObject lightVisual;

    public override void OnPowerOn()
    {
        if (lightVisual != null) lightVisual.SetActive(true);
    }

    public override void OnPowerOff()
    {
        if (lightVisual != null) lightVisual.SetActive(false);
    }
}
