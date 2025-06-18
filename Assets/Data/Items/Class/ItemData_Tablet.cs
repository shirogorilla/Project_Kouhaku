using UnityEngine;

[CreateAssetMenu(fileName = "Tablet", menuName = "Items/Tablet")]
public class ItemData_Tablet : ItemData
{
    private void OnEnable()
    {
        itemType = ItemType.Tablet;
        maxStackAmount = 1; // 1‚Â‚¾‚¯‚Ä‚é
    }

    public override void Use()
    {
        TabletUIManager.Instance?.OpenTablet();
    }
}