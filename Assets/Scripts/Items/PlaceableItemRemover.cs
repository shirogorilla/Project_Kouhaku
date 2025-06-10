using UnityEngine;

public class PlaceableItemRemover : MonoBehaviour, IPlaceableItem
{
    [SerializeField] private ItemData itemData;
    public ItemData ItemData => itemData;

    public void OnLongPressRemove()
    {
        Debug.Log($"{itemData.itemName} を撤去");
        Destroy(gameObject); // 必要ならアニメやSEも追加
        InventoryManager.Instance.AddItem(itemData); // インベントリに戻す処理
    }
}
