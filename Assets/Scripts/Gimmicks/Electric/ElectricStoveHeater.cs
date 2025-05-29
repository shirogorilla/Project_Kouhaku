using Unity.VisualScripting;
using UnityEngine;

public class ElectricStoveHeater : RoomHeater
{
    private ElectricStove stove;

    private void Awake()
    {
        stove = GetComponent<ElectricStove>();
    }

    public override float GetHeatingPower()
    {
        return (stove != null && stove.IsOnHeater()) ? heatPower : 0f;
    }

    private void OnTriggerEnter(Collider other)
    {
        RoomTemperature room = other.GetComponent<RoomTemperature>();
        if (room != null)
        {
            room.RegisterHeater(this);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        RoomTemperature room = other.GetComponent<RoomTemperature>();
        if (room != null)
        {
            room.UnregisterHeater(this);
        }
    }
}
