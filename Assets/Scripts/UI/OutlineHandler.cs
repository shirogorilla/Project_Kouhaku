using UnityEngine;
using QuickOutline;

[RequireComponent(typeof(Outline))]
public class OutlineHandler : MonoBehaviour
{
    public float showDistance = 3f;
    public LayerMask visibilityCheckMask; // 障害物を判定するレイヤーマスク（例: Wall, Default など）

    private Transform player;
    private Camera playerCamera;
    private Outline outline;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        playerCamera = Camera.main;
        outline = GetComponent<Outline>();
        if (outline != null) outline.enabled = false;
    }

    void Update()
    {
        if (player == null || outline == null || playerCamera == null) return;

        float distance = Vector3.Distance(transform.position, player.position);
        if (distance > showDistance)
        {
            outline.enabled = false;
            return;
        }

        // カメラの中心からこのオブジェクトへレイを飛ばす
        Vector3 dir = (transform.position - playerCamera.transform.position).normalized;
        Ray ray = new Ray(playerCamera.transform.position, dir);

        // レイが最初に当たったものがこのオブジェクト自身であれば「見えている」と判断
        if (Physics.Raycast(ray, out RaycastHit hit, showDistance, visibilityCheckMask))
        {
            if (hit.transform == transform)
            {
                outline.enabled = true;
                return;
            }
        }

        outline.enabled = false;
    }
}
