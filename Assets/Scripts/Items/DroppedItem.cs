using UnityEngine;

public class DroppedItem : MonoBehaviour
{
    private ItemData item;

    public void SetItem(ItemData newItem)
    {
        item = newItem;
        // �A�C�R���\���A�F�A���O�Ȃǐݒ肵������΂�����
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
