using UnityEngine;

public class PlayerHitBox : MonoBehaviour
{
    public int damage = 10;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Debug.Log($"バット命中！ {other.name} に {damage} ダメージ");
            // 敵のダメージ処理呼び出し
            var enemy = other.GetComponent<YukidarumaAI>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
        }
    }
}
