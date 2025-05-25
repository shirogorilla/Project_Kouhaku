using UnityEngine;

[CreateAssetMenu(fileName = "WoodenPlank", menuName = "Items/WoodenPlank")]
public class ItemData_WoodenPlank : ItemData
{
    private void Reset()
    {
        itemName = "”Â";
        description = "‘‹‚ð•â‹­‚·‚é‚½‚ß‚Ì–Ø‚Ì”Â";
        itemType = ItemType.WoodenPlank;
        maxStackAmount = 3;
    }
}
