using UnityEngine;

public class PlaceableItemRemover : MonoBehaviour, IPlaceableItem
{
    [SerializeField] private ItemData itemData;
    public ItemData ItemData => itemData;

    public void OnLongPressRemove()
    {
        Debug.Log($"{itemData.itemName} ��P��");
        Destroy(gameObject); // �K�v�Ȃ�A�j����SE���ǉ�
        InventoryManager.Instance.AddItem(itemData); // �C���x���g���ɖ߂�����
    }
}
