using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    [Header("UI References")]
    public GameObject inventoryPanel;
    public GameObject inventorySlotPrefab;

    [Header("Settings")]
    public int defaultSlotCount = 8;

    public GameObject droppedItemPrefab;
    public Transform playerTransform;

    private InventorySlotUI[] slots;
    private int selectedSlotIndex = -1;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        InitializeSlots(defaultSlotCount);
    }

    private void InitializeSlots(int count)
    {
        // 古いスロットがあれば削除
        foreach (Transform child in inventoryPanel.transform)
        {
            Destroy(child.gameObject);
        }

        slots = new InventorySlotUI[count];

        for (int i = 0; i < count; i++)
        {
            GameObject slotObj = Instantiate(inventorySlotPrefab, inventoryPanel.transform);
            InventorySlotUI slotUI = slotObj.GetComponent<InventorySlotUI>();
            slots[i] = slotUI;
        }
    }

    public void AddItem(ItemData item)
    {
        // 【1】バッグだったら、スロット拡張して終了
        if (item.itemType == ItemType.Bag)
        {
            ExpandInventory(2); // 例：2枠増やす
            Debug.Log($"バッグ使用：スロットが拡張されました");
            return;
        }

        // 【2】既存スロットにスタック可能かチェック
        foreach (var slot in slots)
        {
            if (slot.IsSameItem(item) && slot.GetAmount() < item.maxStackAmount)
            {
                if (slot.AddAmount(1))
                {
                    Debug.Log($"既存スタックに追加：{item.itemName}");
                    return;
                }
            }
        }

        // 【3】空きスロットに新規追加
        foreach (var slot in slots)
        {
            if (slot.IsEmpty())
            {
                slot.SetSlot(item, 1);
                Debug.Log($"新規スロットに追加：{item.itemName}");

                // 初回選択がまだならここで選択開始
                if (selectedSlotIndex == -1)
                {
                    int index = System.Array.IndexOf(slots, slot);
                    SelectSlot(index);
                }

                return;
            }
        }

        // 【4】入らなかった場合
        Debug.LogWarning($"インベントリ満杯：{item.itemName} を拾えませんでした");
    }

    public void ExpandInventory(int additionalSlots)
    {
        int oldCount = slots.Length;
        int newCount = oldCount + additionalSlots;

        InventorySlotUI[] newSlots = new InventorySlotUI[newCount];

        // 既存スロットをコピー
        for (int i = 0; i < oldCount; i++)
        {
            newSlots[i] = slots[i];
        }

        // 新しいスロットを生成
        for (int i = oldCount; i < newCount; i++)
        {
            GameObject slotObj = Instantiate(inventorySlotPrefab, inventoryPanel.transform);
            InventorySlotUI slotUI = slotObj.GetComponent<InventorySlotUI>();
            newSlots[i] = slotUI;
        }

        slots = newSlots;
        Debug.Log($"インベントリ拡張：{oldCount} → {newCount} スロット");
    }

    public void SelectNext()
    {
        SelectRelative(1);
    }

    public void SelectPrevious()
    {
        SelectRelative(-1);
    }

    private void SelectRelative(int direction)
    {
        if (slots == null || slots.Length == 0) return;

        // 初回未選択なら強制的に先頭を探す
        if (selectedSlotIndex == -1)
        {
            for (int i = 0; i < slots.Length; i++)
            {
                if (!slots[i].IsEmpty())
                {
                    SelectSlot(i);
                    return;
                }
            }
            return; // 全スロット空の場合
        }

        int newIndex = selectedSlotIndex;
        for (int i = 0; i < slots.Length; i++)
        {
            newIndex = (newIndex + direction + slots.Length) % slots.Length;
            if (!slots[newIndex].IsEmpty())
            {
                SelectSlot(newIndex);
                break;
            }
        }
    }

    private void SelectSlot(int index)
    {
        if (selectedSlotIndex >= 0 && selectedSlotIndex < slots.Length)
            slots[selectedSlotIndex].SetSelected(false);

        selectedSlotIndex = index;
        slots[selectedSlotIndex].SetSelected(true);
    }

    public void UseSelectedItem()
    {
        if (selectedSlotIndex < 0 || selectedSlotIndex >= slots.Length) return;

        var slot = slots[selectedSlotIndex];
        if (slot.IsEmpty()) return;

        var item = slot.GetItem();

        // Usable アイテムのみ Use() を呼び出す
        if (item.itemType != ItemType.Usable)
        {
            Debug.Log($"使用不可タイプ: {item.itemName} は Usable ではありません");
            return;
        }

        Debug.Log($"アイテム使用: {item.itemName}");

        // TODO: アイテムの使用効果（回復、武器発射など）を書く

        // 数を1減らす（AddAmountにマイナスを渡す）
        if (slot.AddAmount(-1))
        {
            if (slot.GetAmount() <= 0)
            {
                slot.ClearSlot();
                CompactInventory();
            }
        }
    }

    public bool ConsumeItem(ItemData item)
    {
        foreach (var slot in slots)
        {
            if (slot.IsSameItem(item) && !slot.IsEmpty())
            {
                bool changed = slot.AddAmount(-1);
                if (slot.GetAmount() <= 0)
                {
                    slot.ClearSlot();
                    CompactInventory();
                }

                if (changed)
                {
                    Debug.Log($"🧱 アイテム消費：{item.itemName}");
                    return true;
                }
            }
        }

        Debug.LogWarning($"❌ 消費失敗：{item.itemName} はインベントリに存在しません");
        return false;
    }

    public void DropSelectedItem()
    {
        if (selectedSlotIndex < 0 || selectedSlotIndex >= slots.Length) return;

        var slot = slots[selectedSlotIndex];
        if (slot.IsEmpty()) return;

        var item = slot.GetItem();
        Debug.Log($"アイテム捨てた: {item.itemName}");

        // 1. 地面にアイテムプレハブを生成
        Vector3 dropPosition = playerTransform.position + playerTransform.forward * 1.5f;// プレイヤー位置の前方にドロップ
        Instantiate(droppedItemPrefab, dropPosition, Quaternion.identity)
            .GetComponent<DroppedItem>().SetItem(item); // アイテムデータ渡す（下で作成）

        // 2. スロット内の数を減らす or 空にする
        if (slot.AddAmount(-1))
        {
            if (slot.GetAmount() <= 0)
            {
                slot.ClearSlot();
                CompactInventory();
            }
        }
    }

    public void CompactInventory()
    {
        List<(ItemData item, int amount)> tempList = new List<(ItemData, int)>();

        // 有効なスロットの情報だけ集める
        foreach (var slot in slots)
        {
            if (!slot.IsEmpty())
            {
                tempList.Add((slot.GetItem(), slot.GetAmount()));
            }
        }

        // すべてのスロットをクリア（empty状態に）
        foreach (var slot in slots)
        {
            slot.ClearSlot();
        }

        // 左詰めで再配置
        for (int i = 0; i < tempList.Count; i++)
        {
            slots[i].SetSlot(tempList[i].item, tempList[i].amount);
        }

        Debug.Log("インベントリ整理（左詰め）完了");
    }

    public ItemData GetSelectedItem()
    {
        if (selectedSlotIndex >= 0 && selectedSlotIndex < slots.Length)
        {
            return slots[selectedSlotIndex].GetItem();
        }
        return null;
    }
}
