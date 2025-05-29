using UnityEngine;

public class ElectricStove : ElectricAppliance, IInteractable
{
    public bool IsOnHeater() => this.IsOn;
    private float holdTime = 2.0f;
    private float holdTimer = 0f;

    private InteractableItem item;

    private void Awake()
    {
        item = GetComponent<InteractableItem>();
    }

    public override void OnPowerOn()
    {
        Debug.Log("電気ストーブ ON");
        // ここでアニメーションやサウンドなど起動演出
    }

    public override void OnPowerOff()
    {
        Debug.Log("電気ストーブ OFF");
        // ここで停止演出（音、アニメーション停止など）
    }

    public void Interact()
    {
        if (this.IsOn)
            this.TurnOff();
        else
            this.TurnOn();
    }

    public void CancelInteract() { }

    public void HoldInteract()
    {
        holdTimer += Time.deltaTime;

        if (holdTimer >= holdTime)
        {
            ReturnToInventory();
        }
    }

    private void ReturnToInventory()
    {
        Debug.Log("🧺 電気ストーブを回収 → アイテムスロットへ");
        item.Interact();
    }
}
