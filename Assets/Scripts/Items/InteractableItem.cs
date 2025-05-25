using UnityEngine;

public class InteractableItem : MonoBehaviour, IInteractable
{
    public ItemData itemData;

    // インターフェース実装（名前は Interact に変更）
    public void Interact()
    {
        if (itemData != null)
        {
            InventoryManager.Instance.AddItem(itemData);
            Destroy(gameObject); // 取得後は削除
        }
        else
        {
            Debug.LogWarning("ItemData が設定されていません。");
        }
    }

    public void CancelInteract()
    {
        // 空処理
    }
}
