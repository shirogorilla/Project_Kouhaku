using System.Collections.Generic;
using UnityEngine;

public class RoomTemperature : MonoBehaviour
{
    public float temperature = 20f;

    [Header("ó‚ãpä÷òA")]
    [SerializeField] private float coolingPerWindow = 0.1f;
    private int openWindowCount = 0;

    [Header("ê⁄ë±")]
    public List<DoorController> connectedDoors;
    public float OutsideTemperature => GameManager.Instance.OutsideTemperature;

    private void Update()
    {
        ApplyWindowCooling();
        ApplyDoorDiffusion();
    }

    private void ApplyWindowCooling()
    {
        if (openWindowCount <= 0) return;

        float outsideTemp = GameManager.Instance?.OutsideTemperature ?? -10f;
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

    public void AddOpenWindow() => openWindowCount++;
    public void RemoveOpenWindow() => openWindowCount = Mathf.Max(0, openWindowCount - 1);

    public void ApplyDiffusion(RoomTemperature otherRoom, float rate)
    {
        float delta = (otherRoom.temperature - temperature) * rate;
        temperature += delta;
        otherRoom.temperature -= delta;
    }
}
