using UnityEngine;

[CreateAssetMenu(fileName = "WoodenPlank", menuName = "Items/WoodenPlank")]
public class ItemData_WoodenPlank : ItemData
{
    private void Reset()
    {
        itemName = "��";
        description = "����⋭���邽�߂̖؂̔�";
        itemType = ItemType.WoodenPlank;
        maxStackAmount = 3;
    }
}
