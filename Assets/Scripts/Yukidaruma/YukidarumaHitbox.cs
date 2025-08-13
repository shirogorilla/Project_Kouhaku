using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YukidarumaHitbox : MonoBehaviour
{
    public float damage = 10f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerStatus status = other.GetComponent<PlayerStatus>();
            if (status != null)
            {
                status.TakeDamage(damage);
                Debug.Log("☃️ 雪霊の攻撃がヒット！");
            }
        }
    }
}
