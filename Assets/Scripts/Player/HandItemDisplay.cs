using UnityEngine;

public class HandItemDisplay : MonoBehaviour
{
    [SerializeField] private Transform displayAnchor; // HandDisplayAnchor をアサイン
    [SerializeField] private InventoryManager inventoryManager;           // ホットバー参照
    private GameObject currentDisplay;

    private int lastSelectedIndex = -1;

    void Update()
    {
        int currentIndex = InventoryManager.Instance.GetSelectedSlotIndex();

        if (currentIndex != lastSelectedIndex)
        {
            UpdateDisplayedItem(currentIndex);
            lastSelectedIndex = currentIndex;
        }
    }

    void UpdateDisplayedItem(int index)
    {
        ItemData selectedItem = InventoryManager.Instance.GetSelectedItem();
        if (selectedItem == null || selectedItem.handPrefab == null)
        {
            if (currentDisplay != null) Destroy(currentDisplay);
            currentDisplay = null;
            return;
        }

        if (currentDisplay == null || currentDisplay.name != selectedItem.handPrefab.name + "(Clone)")
        {
            if (currentDisplay != null) Destroy(currentDisplay);

            currentDisplay = Instantiate(selectedItem.handPrefab, displayAnchor);
            currentDisplay.transform.localPosition = Vector3.zero;
            currentDisplay.transform.localRotation = Quaternion.identity;
        }
    }
}
