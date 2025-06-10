using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySlotUI : MonoBehaviour
{
    public Sprite emptyIcon;
    public Image iconImage;
    public TextMeshProUGUI amountText;

    private ItemData item;
    private int amount;
    private Outline outline;

    [SerializeField] private GameObject fuelGaugeRoot;
    [SerializeField] private Slider fuelGaugeSlider;
    private float fuelAmount; // 現在量
    private float fuelMax;    // 最大量

    private void Awake()
    {
        // Outline は iconImage のゲームオブジェクトから取得する
        if (iconImage != null)
        {
            outline = iconImage.GetComponent<Outline>();
        }
        if (outline != null)
        {
            outline.enabled = false;
        }
    }

    public void SetSlot(ItemData newItem, int newAmount, float fuelAmount = -1f, float fuelMax = -1f)
    {
        item = newItem;
        amount = newAmount;

        iconImage.sprite = item.icon;
        iconImage.enabled = true;
        amountText.text = amount > 0 ? amount.ToString() : "";

        // PlasticFuelCanの場合
        if (item is ItemData_PlasticFuelCan fuelCan)
        {
            fuelGaugeRoot.SetActive(true);
            this.fuelAmount = fuelCan.CurrentAmount;
            this.fuelMax = fuelCan.maxFuelAmount;
            UpdateFuelGauge();
        }
        else
        {
            fuelGaugeRoot.SetActive(false);
        }
    }

    private void UpdateFuelGauge()
    {
        if (fuelGaugeSlider != null && fuelMax > 0f)
        {
            fuelGaugeSlider.minValue = 0f;
            fuelGaugeSlider.maxValue = fuelMax;
            fuelGaugeSlider.value = fuelAmount;
        }
    }

    public void ClearSlot()
    {
        item = null;
        amount = 0;

        iconImage.sprite = emptyIcon;      // ← empty画像を設定
        iconImage.enabled = true;          // ← 表示状態にしておく
        amountText.text = "0";            // ← "0" を表示
    }

    public void SetSelected(bool selected)
    {
        if (outline != null)
            outline.enabled = selected;
    }

    public bool IsSameItem(ItemData otherItem) => item == otherItem;
    public bool IsEmpty() => item == null;
    public int GetAmount() => amount;
    public int MaxAmount => item != null ? item.maxStackAmount : 0;

    public bool AddAmount(int add)
    {
        if (item == null) return false;

        if (amount + add <= MaxAmount)
        {
            amount += add;
            amountText.text = amount > 0 ? amount.ToString() : "";
            return true;
        }

        return false; // オーバー
    }

    public ItemData GetItem()
    {
        return item;
    }
}
