using UnityEngine;

public class FuelGenerator : MonoBehaviour, IInteractable
{
    [SerializeField] private int maxFuel = 100;
    private int currentFuel = 0;

    private bool isFilling = false;
    private float fillTimer = 0f;
    private float fillRate = 0.2f;

    [SerializeField] private float powerIncreaseAmount = 250f;
    private bool isOn = false;
    private float burnTimer = 0f;
    private float burnRate = 5f;

    private ItemData_PlasticFuelCan currentCan;

    public int GetFuelAmount() => currentFuel;
    public int GetMaxFuel() => maxFuel;
    public bool IsRunning() => isOn;

    private void Update()
    {
        HandleFuelFilling();
        HandleFuelConsumption();
    }

    private void HandleFuelFilling()
    {
        if (isFilling && currentCan != null && currentCan.CurrentAmount > 0 && currentFuel < maxFuel)
        {
            fillTimer += Time.deltaTime;

            if (fillTimer >= fillRate)
            {
                fillTimer = 0f;
                currentCan.ConsumeUnit();
                currentFuel++;

                Debug.Log($"🔋 発電機に補充: 現在の燃料 = {currentFuel}/{maxFuel}");

                if (currentFuel >= maxFuel || currentCan.CurrentAmount <= 0)
                {
                    Debug.Log("🔋 補充完了（満タン or ポリタンク空）");
                    StopFilling();
                }
            }
        }
    }

    private void HandleFuelConsumption()
    {
        if (isOn && currentFuel > 0)
        {
            burnTimer += Time.deltaTime;

            if (burnTimer >= burnRate)
            {
                burnTimer = 0f;
                currentFuel--;

                Debug.Log($"🔋 発電中: 燃料残り {currentFuel}");

                if (currentFuel <= 0)
                {
                    Debug.Log("🔋 燃料切れ！発電機自動OFF");
                    TurnOff();
                }
            }
        }
    }

    public void Interact()
    {
        if (!isFilling)
        {
            if (isOn)
            {
                TurnOff();
            }
            else
            {
                if (currentFuel > 0)
                {
                    TurnOn();
                }
                else
                {
                    Debug.Log("🔋 燃料がないため発電できません");
                }
            }
        }
    }

    private void TurnOn()
    {
        isOn = true;
        burnTimer = 0f;
        PowerManager.Instance?.AddPowerCapacity(powerIncreaseAmount);
        Debug.Log("🔋 発電機 ON（容量増加）");
    }

    private void TurnOff()
    {
        isOn = false;
        PowerManager.Instance?.RemovePowerCapacity(powerIncreaseAmount);
        Debug.Log("🛑 発電機 OFF（容量減少）");
    }

    public void StartFillingExternally()
    {
        var selected = InventoryManager.Instance.GetSelectedItem();
        if (selected?.itemType == ItemType.PlasticFuelCan && selected is ItemData_PlasticFuelCan can)
        {
            currentCan = can;
        }
        else
        {
            currentCan = null;
        }

        if (currentCan != null && currentCan.CurrentAmount > 0 && currentFuel < maxFuel)
        {
            isFilling = true;
            fillTimer = 0f;
            Debug.Log("🔋 補充開始（長押し）");
        }
        else
        {
            Debug.Log("🔋 補充できません：空 or 満タン");
        }
    }

    public void CancelInteract()
    {
        Debug.Log("🔋 補充キャンセル");
        StopFilling();
    }

    private void StopFilling()
    {
        isFilling = false;
        fillTimer = 0f;
    }

    public int GetCurrentFuel() => currentFuel;
    public bool IsOn() => isOn;
}
