using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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

    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Material previewMaterial;
    [SerializeField] private Material cannotPlaceMaterial;
    [SerializeField] private float maxPlaceDistance = 5f; // 設置可能距離
    private bool canPlaceHere = false; // 現在の設置可否状態
    private Dictionary<Renderer, Material[]> originalMaterials = new Dictionary<Renderer, Material[]>();

    private InventorySlotUI[] slots;
    private int selectedSlotIndex = -1;

    private GameObject previewObject = null;
    private ItemData placingItem = null;
    private InventorySlotUI placingSlot = null;
    public bool IsPlacingItem()
    {
        return previewObject != null;
    }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        InitializeSlots(defaultSlotCount);
    }

    private void Update()
    {
        if (IsPlacingItem())
        {
            UpdatePreviewPosition();
        }
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

    private void UpdatePreviewPosition()
    {
        if (Camera.main == null || previewObject == null) return;

        Vector2 mousePos = Mouse.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(mousePos);

        if (Physics.Raycast(ray, out RaycastHit hit, 100f, groundLayer))
        {
            previewObject.transform.position = hit.point;

            float distance = Vector3.Distance(playerTransform.position, hit.point);

            if (distance <= maxPlaceDistance)
            {
                // 設置可能
                if (!canPlaceHere)
                {
                    SetPreviewMaterial(previewMaterial);
                    canPlaceHere = true;
                }
            }
            else
            {
                // 設置不可 (遠すぎる)
                if (canPlaceHere)
                {
                    SetPreviewMaterial(cannotPlaceMaterial);
                    canPlaceHere = false;
                }
            }
        }
        else
        {
            // RayがGroundLayerに当たってない → 設置不可
            if (canPlaceHere)
            {
                SetPreviewMaterial(cannotPlaceMaterial);
                canPlaceHere = false;
            }
        }
    }

    private void SetPreviewMaterial(Material mat)
    {
        foreach (var renderer in previewObject.GetComponentsInChildren<Renderer>())
        {
            Material[] mats = new Material[renderer.sharedMaterials.Length];
            for (int i = 0; i < mats.Length; i++)
            {
                mats[i] = mat;
            }
            renderer.materials = mats;
        }
    }

    public void ConfirmPlacement()
    {
        if (placingItem == null || previewObject == null || placingSlot == null) return;

        if (!canPlaceHere)
        {
            Debug.Log("❌ この位置には設置できません！");
            return;
        }

        Instantiate(placingItem.placeablePrefab, previewObject.transform.position, previewObject.transform.rotation);
        placingSlot.AddAmount(-1);

        if (placingSlot.GetAmount() <= 0)
        {
            placingSlot.ClearSlot();
            CompactInventory();
        }

        CancelPlacement();
    }

    public void CancelPlacement()
    {
        if (previewObject != null)
        {
            Destroy(previewObject);
            previewObject = null;
        }

        placingItem = null;
        placingSlot = null;
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

        if (placingItem != null)
        {
            // すでに設置待ち状態なら再度の使用は確定設置
            ConfirmPlacement();
            return;
        }

        switch (item.itemType)
        {
            case ItemType.Usable:
                Debug.Log($"アイテム使用: {item.itemName}");
                // TODO: アイテムの効果を呼び出す
                if (slot.AddAmount(-1))
                {
                    if (slot.GetAmount() <= 0)
                    {
                        slot.ClearSlot();
                        CompactInventory();
                    }
                }
                break;

            case ItemType.Placeable:
                Debug.Log($"設置モード開始: {item.itemName}");
                StartPlacement(item, slot);
                break;

            default:
                Debug.Log($"使用不可タイプ: {item.itemName} は Usable または Placeable ではありません");
                break;
        }
    }

    private void StartPlacement(ItemData item, InventorySlotUI slot)
    {
        if (previewObject != null) Destroy(previewObject);

        previewObject = Instantiate(item.placeablePrefab);
        SetPreviewMode(previewObject, true);

        placingItem = item;
        placingSlot = slot;
    }

    private void SetPreviewMode(GameObject obj, bool isPreview)
    {
        foreach (var renderer in obj.GetComponentsInChildren<Renderer>())
        {
            if (isPreview)
            {
                // 元のMaterial保存（初回だけ）
                if (!originalMaterials.ContainsKey(renderer))
                {
                    originalMaterials[renderer] = renderer.sharedMaterials;
                }

                // 全サブメッシュに PreviewMaterial をセット
                Material[] previewMats = new Material[renderer.sharedMaterials.Length];
                for (int i = 0; i < previewMats.Length; i++)
                {
                    previewMats[i] = previewMaterial;
                }
                renderer.materials = previewMats;
            }
            else
            {
                // 元のMaterialに戻す
                if (originalMaterials.TryGetValue(renderer, out var mats))
                {
                    renderer.materials = mats;
                }
            }
        }

        // Collider無効化
        if (obj.TryGetComponent<Collider>(out var col))
        {
            col.enabled = !isPreview ? true : false;
        }

        // Cleanup
        if (!isPreview)
        {
            originalMaterials.Clear();
        }
    }

    public void UseItemPerformed()
    {

    }

    public void UseItemCanceled()
    {

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

    public void OnItemPlaced(ItemData item, InventorySlotUI sourceSlot)
    {
        Debug.Log($"{item.itemName} を設置完了");

        if (sourceSlot.AddAmount(-1))
        {
            if (sourceSlot.GetAmount() <= 0)
            {
                sourceSlot.ClearSlot();
                CompactInventory();
            }
        }
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
