using UnityEngine;

public class BatController : MonoBehaviour
{
    public static BatController Instance;

    private Animator animator;
    private bool isAttacking = false;

    private void Awake()
    {
        Instance = this;
        animator = GetComponentInChildren<Animator>(); // バットモデルに Animator がある想定
    }

    public void Attack(ItemData_Bat batData)
    {
        if (isAttacking) return; // 連打防止
        isAttacking = true;

        // アニメーション再生
        animator.SetTrigger("Attack");

        // ヒットボックス生成
        StartCoroutine(SpawnHitbox(batData));
    }

    private System.Collections.IEnumerator SpawnHitbox(ItemData_Bat batData)
    {
        yield return new WaitForSeconds(0.1f); // アニメの振り抜きタイミングに合わせて調整

        var hitbox = Instantiate(
            batData.hitboxPrefab,
            transform.position + transform.forward * 1.0f, // プレイヤー前方
            transform.rotation
        );
        hitbox.transform.SetParent(transform); // プレイヤーに追従（必要なら外す）

        Destroy(hitbox, batData.hitboxActiveTime);

        // 攻撃終了待機
        yield return new WaitForSeconds(batData.attackDuration);
        isAttacking = false;
    }
}
