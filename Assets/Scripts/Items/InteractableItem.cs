using UnityEngine;

public class InteractableItem : MonoBehaviour, IInteractable
{
    public ItemData itemData;

    // �C���^�[�t�F�[�X�����i���O�� Interact �ɕύX�j
    public void Interact()
    {
        if (itemData != null)
        {
            InventoryManager.Instance.AddItem(itemData);
            Destroy(gameObject); // �擾��͍폜
        }
        else
        {
            Debug.LogWarning("ItemData ���ݒ肳��Ă��܂���B");
        }
    }

    public void CancelInteract()
    {
        // �󏈗�
    }
}
