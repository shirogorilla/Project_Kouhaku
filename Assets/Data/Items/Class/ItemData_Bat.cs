using UnityEngine;

[CreateAssetMenu(fileName = "Bat", menuName = "Items/Bat")]
public class ItemData_Bat : ItemData
{
    public override void Use()
    {
        // UŒ‚ˆ—ŠJn
        BatController.Instance.Attack();
    }
}
