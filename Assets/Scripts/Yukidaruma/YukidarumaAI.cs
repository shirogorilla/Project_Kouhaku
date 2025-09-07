using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class YukidarumaAI : MonoBehaviour
{
    [Header("Chase Settings")]
    public float chaseRange = 10f;       // プレイヤーを追跡する範囲
    public float stopDistance = 2f;      // 攻撃に入る距離

    [Header("Attack Settings")]
    [SerializeField] private GameObject attackHitbox; // 攻撃判定のヒットボックス
    [SerializeField] private float attackCooldown = 2.0f; // 攻撃間隔
    [SerializeField] private float attackDamage = 10f;    // 与ダメージ
    private float lastAttackTime = -Mathf.Infinity;
    private bool isAttacking = false; // 攻撃中かどうか

    [SerializeField] private float baseSpeed = 1.0f; // 基本速度
    [SerializeField] private float updateTempInterval = 1.0f; // 温度チェック間隔(秒)
    private RoomTemperature currentRoom;
    private float nextTempCheckTime = 0f;

    private NavMeshAgent agent;
    private Transform player;
    private GameObject[] entryPoints;
    private Transform currentTargetEntry;
    private Animator animator;

    private enum State { GoToEntryPoint, ChasePlayer, Idle }
    private State currentState = State.GoToEntryPoint;

    private bool hasEnteredHouse = false;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        if (attackHitbox != null)
        {
            attackHitbox.SetActive(false); // 初期は非アクティブ

            // 攻撃力を渡す
            var hitbox = attackHitbox.GetComponent<YukidarumaHitbox>();
            if (hitbox != null)
            {
                hitbox.Initialize(attackDamage);
            }
        }

        // プレイヤーの Transform を取得
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }

        // 侵入地点を取得
        entryPoints = GameObject.FindGameObjectsWithTag("EntryPoint");
        currentTargetEntry = FindClosestEntryPoint();

        if (currentTargetEntry != null)
        {
            agent.SetDestination(currentTargetEntry.position);
        }

        // WalkType を個体ごとに決定
        int walkType = Random.Range(0, 3);
        animator.SetInteger("WalkType", walkType);
        animator.Play("Walk" + walkType, 0, 0f);
    }

    private void Update()
    {
        if (isAttacking) return; // 攻撃中はAI制御を止める

        // 温度チェック（定期的）
        if (Time.time >= nextTempCheckTime && currentRoom != null)
        {
            nextTempCheckTime = Time.time + updateTempInterval;
            ApplyTemperatureEffect();
        }

        switch (currentState)
        {
            case State.GoToEntryPoint:
                if (currentTargetEntry != null &&
                    Vector3.Distance(transform.position, currentTargetEntry.position) < 1.5f)
                {
                    EnterHouse();
                }
                break;

            case State.ChasePlayer:
                if (player == null) return;

                float distance = Vector3.Distance(transform.position, player.position);

                if (distance > chaseRange)
                {
                    currentState = State.Idle;
                    agent.ResetPath();
                }
                else if (distance > stopDistance)
                {
                    agent.isStopped = false;
                    agent.SetDestination(player.position);
                    CheckAndForceOpenDoor();
                }
                else
                {
                    // 攻撃判定
                    agent.ResetPath();
                    agent.isStopped = true;

                    if (Time.time >= lastAttackTime + attackCooldown)
                    {
                        lastAttackTime = Time.time;
                        isAttacking = true;

                        int attackType = Random.Range(0, 3);
                        animator.SetInteger("AttackType", attackType);
                        animator.SetTrigger("Attack");
                    }
                }
                break;

            case State.Idle:
                if (player == null) return;

                if (Vector3.Distance(transform.position, player.position) < chaseRange)
                {
                    currentState = State.ChasePlayer;
                }
                break;
        }
    }

    private Transform FindClosestEntryPoint()
    {
        Transform closest = null;
        float minDistance = Mathf.Infinity;

        foreach (GameObject point in entryPoints)
        {
            float distance = Vector3.Distance(transform.position, point.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closest = point.transform;
            }
        }
        return closest;
    }

    private void EnterHouse()
    {
        if (hasEnteredHouse) return;
        hasEnteredHouse = true;

        Window window = currentTargetEntry.GetComponent<Window>();
        if (window != null && !window.IsPassable())
        {
            window.BreakWindow();
            StartCoroutine(WaitForWindowBreak(window));
            return;
        }

        currentState = State.Idle;
    }

    private IEnumerator WaitForWindowBreak(Window window)
    {
        while (!window.IsPassable())
        {
            yield return null;
        }

        Debug.Log("🧊 雪霊が窓を破壊して侵入しました");
        currentState = State.Idle;
    }

    private void CheckAndForceOpenDoor()
    {
        RaycastHit hit;
        Vector3 origin = transform.position + Vector3.up * 0.5f;
        Vector3 direction = transform.forward;
        float checkDistance = 1.0f;

        if (Physics.Raycast(origin, direction, out hit, checkDistance))
        {
            var door = hit.collider.GetComponentInParent<DoorController>();
            if (door != null)
            {
                door.ForceOpen(transform.position);
            }
        }
    }

    // ===== 攻撃アニメーションイベントで呼ばれる =====
    public void EnableHitbox()
    {
        if (attackHitbox != null)
        {
            attackHitbox.SetActive(true);
            StartCoroutine(DisableHitboxAfterDelay(0.3f));
        }
    }

    public void OnAttackEnd()
    {
        if (agent != null)
        {
            agent.isStopped = false;
        }
        isAttacking = false;
    }

    private IEnumerator DisableHitboxAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (attackHitbox != null)
        {
            attackHitbox.SetActive(false);
        }
    }

    private void ApplyTemperatureEffect()
    {
        float speedMultiplier = currentRoom.GetSpeedMultiplier();
        agent.speed = baseSpeed * speedMultiplier;
    }

    private void OnTriggerEnter(Collider other)
    {
        RoomTemperature room = other.GetComponent<RoomTemperature>();
        if (room != null)
        {
            currentRoom = room;
            ApplyTemperatureEffect(); // 入室直後に即反映
        }
    }
}
