using UnityEngine;

public enum ItemType
{
    Usable,         // 使用可能アイテム
    Placeable,      // 設置可能アイテム
    Bag,            // ナップサック(アイテムスロット拡張用)
    PlasticFuelCan, // ポリタンク(燃料持ち運び用)
    WoodenPlank,    // 板(窓の補強用)
    KeyItem,        // 鍵アイテム（今後拡張用）
}

[CreateAssetMenu(fileName = "New Item", menuName = "Items/Item")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public string description;
    public int maxStackAmount = 10;  // 最大所持数
    public ItemType itemType = ItemType.Usable;

    public GameObject placeablePrefab;

    public int maxFuelAmount;
}
