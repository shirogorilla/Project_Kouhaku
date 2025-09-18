using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct TempSpeedPoint
{
    public float temperature;   // 温度
    public float speedMultiplier; // 倍率
}

public class RoomTemperature : MonoBehaviour
{
    public float currentTemperature = 20f;

    [SerializeField]
    private TempSpeedPoint[] tempCurve =
    {
        new TempSpeedPoint { temperature = 40f,  speedMultiplier = 0.1f },
        new TempSpeedPoint { temperature = 20f,  speedMultiplier = 0.5f },
        new TempSpeedPoint { temperature = 0f,   speedMultiplier = 1.0f },
        new TempSpeedPoint { temperature = -10f, speedMultiplier = 1.2f },
        new TempSpeedPoint { temperature = -20f, speedMultiplier = 1.5f }
    };

    [Header("冷却関連")]
    [SerializeField] private float coolingPerWindow = 0.1f;
    private int openWindowCount = 0;

    [Header("暖房器具（固定型をInspectorで設定）")]
    [SerializeField] private List<RoomHeater> staticHeaters = new List<RoomHeater>();

    // 移動可能なヒーター（ストーブなど）を実行時に登録管理
    private List<RoomHeater> dynamicHeaters = new List<RoomHeater>();

    [Header("接続")]
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

        currentTemperature = Mathf.Lerp(currentTemperature, outsideTemp, coolingRate * Time.deltaTime);
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
                currentTemperature += heater.GetHeatingPower() * Time.deltaTime;
        }

        foreach (var heater in dynamicHeaters)
        {
            if (heater != null)
                currentTemperature += heater.GetHeatingPower() * Time.deltaTime;
        }
    }

    public void AddOpenWindow() => openWindowCount++;
    public void RemoveOpenWindow() => openWindowCount = Mathf.Max(0, openWindowCount - 1);

    public void ApplyDiffusion(RoomTemperature otherRoom, float rate)
    {
        float delta = (otherRoom.currentTemperature - currentTemperature) * rate;
        currentTemperature += delta;
        otherRoom.currentTemperature -= delta;
    }

    // 移動型暖房器具用
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

    public float GetTemperature()
    {
        return currentTemperature;
    }

    /// <summary>
    /// 現在の温度に基づき速度倍率を返す
    /// </summary>
    public float GetSpeedMultiplier()
    {
        // 上限・下限チェック
        if (currentTemperature >= tempCurve[0].temperature)
            return tempCurve[0].speedMultiplier;
        if (currentTemperature <= tempCurve[tempCurve.Length - 1].temperature)
            return tempCurve[tempCurve.Length - 1].speedMultiplier;

        // 区間を探す
        for (int i = 0; i < tempCurve.Length - 1; i++)
        {
            TempSpeedPoint p1 = tempCurve[i];
            TempSpeedPoint p2 = tempCurve[i + 1];

            if (currentTemperature <= p1.temperature && currentTemperature >= p2.temperature)
            {
                // 線形補間
                float t = Mathf.InverseLerp(p1.temperature, p2.temperature, currentTemperature);
                return Mathf.Lerp(p1.speedMultiplier, p2.speedMultiplier, t);
            }
        }

        // 想定外（ありえないはず）
        return 1f;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerMovement.Instance.SetCurrentRoomTemperature(currentTemperature);
        }
    }
}
