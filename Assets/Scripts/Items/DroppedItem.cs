using UnityEngine;

public class DroppedItem : MonoBehaviour
{
    private ItemData item;

    public void SetItem(ItemData newItem)
    {
        item = newItem;
        // アイコン表示、色、名前など設定したければここで
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            InventoryManager.Instance.AddItem(item);
            Destroy(gameObject);
        }
    }
}
