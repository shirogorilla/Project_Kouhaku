using UnityEngine;

public class LightSwitch : MonoBehaviour, IInteractable
{
    [SerializeField] private LightObject targetLight;

    public void Interact()
    {
        if (targetLight != null)
        {
            if (targetLight.IsOn)
                targetLight.TurnOff();
            else
                targetLight.TurnOn();
        }
    }

    public void CancelInteract() { }
}
