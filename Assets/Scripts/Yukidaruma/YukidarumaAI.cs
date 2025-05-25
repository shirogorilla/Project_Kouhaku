using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class YukidarumaAI : MonoBehaviour
{
    public float chaseRange = 10f;
    public float stopDistance = 2f;

    private NavMeshAgent agent;
    private Transform player;
    private GameObject[] entryPoints; // 侵入地点の配列
    private Transform currentTargetEntry;

    private enum State { GoToEntryPoint, ChasePlayer, Idle }
    private State currentState = State.GoToEntryPoint;

    private bool hasEnteredHouse = false;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        // プレイヤーの Transform をタグで取得（要：プレイヤーに "Player" タグ）
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }

        // 侵入地点をタグで取得（要：侵入地点に "EntryPoint" タグをつける）
        entryPoints = GameObject.FindGameObjectsWithTag("EntryPoint");

        // 最も近い侵入地点を探す
        currentTargetEntry = FindClosestEntryPoint();

        // 最寄りの侵入地点へ移動
        if (currentTargetEntry != null)
        {
            agent.SetDestination(currentTargetEntry.position);
        }
    }

    private void Update()
    {
        switch (currentState)
        {
            case State.GoToEntryPoint:
                if (currentTargetEntry != null && Vector3.Distance(transform.position, currentTargetEntry.position) < 1.5f)
                {
                    // 侵入地点に到達したと判定 → 侵入処理
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
                    agent.SetDestination(player.position);

                    // ↓ ドアチェック追加（前方に何か障害物があれば判定）
                    CheckAndForceOpenDoor();
                }
                else
                {
                    agent.ResetPath(); // 攻撃処理へ
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

        // 侵入地点に Window スクリプトがあれば取得
        Window window = currentTargetEntry.GetComponent<Window>();
        if (window != null)
        {
            if (!window.IsPassable())
            {
                // 窓が通れない状態なら破壊を試みる
                window.BreakWindow();

                // 再侵入を待つ（窓の破壊が完了するまで動かない）
                StartCoroutine(WaitForWindowBreak(window));
                return;
            }
        }

        // 窓がなかった or すでに通れる状態なら進行
        currentState = State.Idle; // 待機しつつ、プレイヤー距離で追跡に入る
    }

    private IEnumerator WaitForWindowBreak(Window window)
    {
        // 窓が壊れるまで待つ（毎フレーム確認）
        while (!window.IsPassable())
        {
            yield return null;
        }

        Debug.Log("🧊 雪霊が窓の破壊を終え、侵入しました");
        currentState = State.Idle;
    }

    private void CheckAndForceOpenDoor()
    {
        RaycastHit hit;
        Vector3 origin = transform.position + Vector3.up * 0.5f; // 胸あたりから
        Vector3 direction = transform.forward;

        float checkDistance = 1.0f; // 距離1mまで

        if (Physics.Raycast(origin, direction, out hit, checkDistance))
        {
            // ドア処理：DoorControllerを持つ親を探す
            var door = hit.collider.GetComponentInParent<DoorController>();
            if (door != null)
            {
                door.ForceOpen(transform.position);
            }
        }
    }
}
