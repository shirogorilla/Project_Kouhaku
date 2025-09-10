using UnityEngine;

public class BatController : MonoBehaviour
{
    public static BatController Instance;

    [SerializeField] private GameObject hitboxObject;

    private Animator animator;
    private bool isAttacking = false;

    private void Awake()
    {
        Instance = this;
        animator = GetComponentInChildren<Animator>();

        if (hitboxObject != null)
            hitboxObject.SetActive(false);
    }

    public void Attack()
    {
        if (isAttacking) return;
        isAttacking = true;
        animator.SetTrigger("Attack");
    }

    // --- アニメーションイベントから呼ぶ ---

    public void EnableHitbox()
    {
        if (hitboxObject != null)
            hitboxObject.SetActive(true);
    }

    public void DisableHitbox()
    {
        if (hitboxObject != null)
            hitboxObject.SetActive(false);

        isAttacking = false;
    }
}
