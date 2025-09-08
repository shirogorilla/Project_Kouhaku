using UnityEngine;

[CreateAssetMenu(fileName = "Bat", menuName = "Items/Bat")]
public class ItemData_Bat : ItemData
{
    public GameObject hitboxPrefab;   // ヒットボックスのプレハブ
    public float attackDuration = 0.5f; // 攻撃アニメの長さ
    public float hitboxActiveTime = 0.2f; // ヒットボックスが有効な時間

    public override void Use()
    {
        // 攻撃処理開始
        BatController.Instance.Attack(this);
    }
}
