using UnityEngine;
using UnityEngine.InputSystem;

public class FuelTank : MonoBehaviour, IInteractable
{
    private bool isFilling = false;
    private float fillTimer = 0f;
    private float fillRate = 0.2f; // 0.2�b���Ƃ�1��[

    private ItemData_PlasticFuelCan currentCan;

    private void Update()
    {
        if (isFilling && currentCan != null && !currentCan.IsFull)
        {
            fillTimer += Time.deltaTime;

            if (fillTimer >= fillRate)
            {
                fillTimer = 0f;
                currentCan.FillUnit(); // 1�P�ʂ�����[

                Debug.Log($"�R����[: ���݂̗� = {currentCan.CurrentAmount}");

                if (currentCan.IsFull)
                {
                    Debug.Log("���^���ɂȂ�܂����I");
                    StopFilling();
                }
            }
        }
    }

    public void Interact()
    {
        // �󏈗�
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

        if (currentCan != null && !currentCan.IsFull)
        {
            isFilling = true;
            fillTimer = 0f;
            Debug.Log("��[�J�n�i�������j");
        }
        else
        {
            Debug.Log("��[�J�n�ł��܂���F�|���^���N���Ȃ� or ���^��");
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
        Debug.Log("��[�J�n");
    }

    private void StopFilling()
    {
        isFilling = false;
        Debug.Log("��[��~");
    }
}
