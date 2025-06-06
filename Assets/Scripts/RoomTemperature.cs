using System.Collections.Generic;
using UnityEngine;

public class RoomTemperature : MonoBehaviour
{
    public float temperature = 20f;

    [Header("âpÖA")]
    [SerializeField] private float coolingPerWindow = 0.1f;
    private int openWindowCount = 0;

    [Header("g[íïiÅè^ðInspectorÅÝèj")]
    [SerializeField] private List<RoomHeater> staticHeaters = new List<RoomHeater>();

    // Ú®Â\Èq[^[iXg[uÈÇjðÀsÉo^Ç
    private List<RoomHeater> dynamicHeaters = new List<RoomHeater>();

    [Header("Ú±")]
    public List<DoorController> connectedDoors;
    public float OutsideTemperature => GameManager.Instance.OutsideTemperature;

    private void Update()
    {
        ApplyWindowCooling();
        ApplyDoorDiffusion();
        ApplyHeating();
    }

    private void ApplyWindowCooling()
    {
        if (openWindowCount <= 0) return;

        float outsideTemp = OutsideTemperature;
        float coolingRate = coolingPerWindow * openWindowCount;

        temperature = Mathf.Lerp(temperature, outsideTemp, coolingRate * Time.deltaTime);
    }

    private void ApplyDoorDiffusion()
    {
        foreach (var door in connectedDoors)
        {
            if (door.IsOpen)
            {
                RoomTemperature other = door.GetOtherRoom(this);
                if (other != null)
                {
                    ApplyDiffusion(other, door.diffusionRate * Time.deltaTime);
                }
            }
        }
    }

    private void ApplyHeating()
    {
        foreach (var heater in staticHeaters)
        {
            if (heater != null)
                temperature += heater.GetHeatingPower() * Time.deltaTime;
        }

        foreach (var heater in dynamicHeaters)
        {
            if (heater != null)
                temperature += heater.GetHeatingPower() * Time.deltaTime;
        }
    }

    public void AddOpenWindow() => openWindowCount++;
    public void RemoveOpenWindow() => openWindowCount = Mathf.Max(0, openWindowCount - 1);

    public void ApplyDiffusion(RoomTemperature otherRoom, float rate)
    {
        float delta = (otherRoom.temperature - temperature) * rate;
        temperature += delta;
        otherRoom.temperature -= delta;
    }

    // Ú®^g[íïp
    public void RegisterHeater(RoomHeater heater)
    {
        if (!dynamicHeaters.Contains(heater))
            dynamicHeaters.Add(heater);
    }

    public void UnregisterHeater(RoomHeater heater)
    {
        if (dynamicHeaters.Contains(heater))
            dynamicHeaters.Remove(heater);
    }
}
