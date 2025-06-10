using UnityEngine;

public class FuelStove : MonoBehaviour, IInteractable
{
    [SerializeField] private int maxFuel = 10;
    private int currentFuel = 0;

    private bool isFilling = false;
    private float fillTimer = 0f;
    private float fillRate = 0.2f;

    private ItemData_PlasticFuelCan currentCan;
    private InventorySlotUI currentSlotUI;

    private bool isOn = false;
    private float burnTimer = 0f;
    private float burnRate = 5f; // 5秒ごとに1消費（好みで調整可能）

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

                // 🔥 スロットUIを更新
                if (currentSlotUI != null)
                {
                    currentSlotUI.SetSlot(currentCan, currentSlotUI.GetAmount());
                }

                Debug.Log($"🔥 ストーブに補充: 現在の燃料 = {currentFuel}/{maxFuel}");

                if (currentFuel >= maxFuel || currentCan.CurrentAmount <= 0)
                {
                    Debug.Log("🔥 補充完了（満タン or ポリタンク空）");
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

                Debug.Log($"🔥 燃料消費: 残り {currentFuel}");

                if (currentFuel <= 0)
                {
                    Debug.Log("🔥 燃料切れ！自動OFF");
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
                    Debug.Log("🔥 燃料がないため電源ONできません");
                }
            }
        }
    }

    private void TurnOn()
    {
        isOn = true;
        burnTimer = 0f;
        Debug.Log("🔥 ストーブ ON");
        // ここでアニメーションやサウンドなど起動演出
    }

    private void TurnOff()
    {
        isOn = false;
        Debug.Log("🧊 ストーブ OFF");
        // ここで停止演出（音、炎アニメーション停止など）
    }

    public void StartFillingExternally()
    {
        var selected = InventoryManager.Instance.GetSelectedItem();
        var selectedSlotUI = InventoryManager.Instance.GetSelectedSlotUI();

        if (selected?.itemType == ItemType.PlasticFuelCan && selected is ItemData_PlasticFuelCan can && selectedSlotUI != null)
        {
            currentCan = can;
            currentSlotUI = selectedSlotUI;
        }
        else
        {
            currentCan = null;
            currentSlotUI = null;
        }

        if (currentCan != null && currentCan.CurrentAmount > 0 && currentFuel < maxFuel)
        {
            isFilling = true;
            fillTimer = 0f;
            Debug.Log("🔥 補充開始（長押し）");
        }
        else
        {
            Debug.Log("🔥 補充できません：空 or 満タン");
        }
    }

    public void CancelInteract()
    {
        Debug.Log("🔥 補充キャンセル");
        StopFilling();
    }

    private void StopFilling()
    {
        isFilling = false;
        fillTimer = 0f;
    }

    public int GetCurrentFuel() => currentFuel;
    public bool IsOn() => isOn;

    // Interact用に、ポリタンクをセットする用
    public void SetCurrentCan(ItemData_PlasticFuelCan can)
    {
        currentCan = can;
    }
}
