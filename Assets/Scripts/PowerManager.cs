using System.Collections.Generic;
using UnityEngine;

public class PowerManager : MonoBehaviour
{
    public static PowerManager Instance { get; private set; }

    [Header("電力容量")]
    [SerializeField] private float basePowerCapacity = 300f; // 初期最大電力
    private float additionalCapacity = 0f; // 発電機等で増えた分
    public float MaxPowerCapacity => basePowerCapacity + additionalCapacity;

    [Header("状態")]
    private float currentPowerUsage = 0f;
    private bool isBreakerTripped = false;

    private readonly List<IElectricDevice> registeredDevices = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Update()
    {
        // ブレーカー監視
        if (!isBreakerTripped && currentPowerUsage > MaxPowerCapacity)
        {
            Debug.Log("⚡ ブレーカーが落ちました！");
            TripBreaker();
        }
    }

    /// <summary>
    /// 現在の消費電力に追加してもキャパシティを超えないか？
    /// </summary>
    public bool CanConsumePower(float amount)
    {
        return !isBreakerTripped && (currentPowerUsage + amount <= MaxPowerCapacity);
    }


    public void RegisterDevice(IElectricDevice device)
    {
        if (!registeredDevices.Contains(device))
            registeredDevices.Add(device);
    }

    public void UnregisterDevice(IElectricDevice device)
    {
        registeredDevices.Remove(device);
    }

    public void ReportPowerUsage(float powerDelta)
    {
        currentPowerUsage += powerDelta;
        currentPowerUsage = Mathf.Max(0f, currentPowerUsage);
    }

    private void TripBreaker()
    {
        isBreakerTripped = true;

        // すべての機器を強制OFFにする
        foreach (var device in registeredDevices)
        {
            device.ForcePowerOff();
        }

        currentPowerUsage = 0f;
    }

    public void ResetBreaker()
    {
        Debug.Log("🔁 ブレーカーを復帰させました");
        isBreakerTripped = false;

        // 注意：復帰時に電化製品の電源はOFFのまま
    }

    public bool IsBreakerTripped() => isBreakerTripped;

    public void AddPowerCapacity(float amount)
    {
        additionalCapacity += amount;
    }

    public void RemovePowerCapacity(float amount)
    {
        additionalCapacity = Mathf.Max(0f, additionalCapacity - amount);
    }

    public float GetCurrentUsage() => currentPowerUsage;
    public float GetMaxCapacity() => MaxPowerCapacity;
}
