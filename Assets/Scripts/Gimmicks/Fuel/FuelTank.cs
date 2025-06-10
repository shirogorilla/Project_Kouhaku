using UnityEngine;
using UnityEngine.InputSystem;

public class FuelTank : MonoBehaviour, IInteractable
{
    private bool isFilling = false;
    private float fillTimer = 0f;
    private float fillRate = 0.2f; // 0.2秒ごとに1補充

    private ItemData_PlasticFuelCan currentCan;
    private InventorySlotUI currentSlotUI;

    private void Update()
    {
        if (isFilling && currentCan != null && !currentCan.IsFull)
        {
            fillTimer += Time.deltaTime;

            if (fillTimer >= fillRate)
            {
                fillTimer = 0f;
                currentCan.FillUnit(); // 1単位だけ補充
                Debug.Log($"燃料補充: 現在の量 = {currentCan.CurrentAmount}");

                // 🔥 スロットUIを更新
                if (currentSlotUI != null)
                {
                    currentSlotUI.SetSlot(currentCan, currentSlotUI.GetAmount());
                }

                if (currentCan.IsFull)
                {
                    Debug.Log("満タンになりました！");
                    StopFilling();
                }
            }
        }
    }

    public void Interact()
    {
        // 空処理
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

        if (currentCan != null && !currentCan.IsFull)
        {
            StartFilling();
        }
        else
        {
            Debug.Log("補充開始できません：ポリタンクがない or 満タン");
        }
    }

    public void CancelInteract()
    {
        StopFilling();
    }

    private void StartFilling()
    {
        isFilling = true;
        fillTimer = 0f;
        Debug.Log("補充開始");
    }

    private void StopFilling()
    {
        isFilling = false;
        Debug.Log("補充停止");
    }
}
