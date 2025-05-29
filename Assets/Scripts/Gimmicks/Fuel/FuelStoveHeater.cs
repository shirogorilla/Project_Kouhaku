using UnityEngine;

public class FuelStoveHeater : RoomHeater
{
    private FuelStove stove;

    private void Awake()
    {
        stove = GetComponent<FuelStove>();
    }

    public override float GetHeatingPower()
    {
        return (stove != null && stove.IsOn()) ? heatPower : 0f;
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