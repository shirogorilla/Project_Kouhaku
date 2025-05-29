using UnityEngine;
using QuickOutline;

[RequireComponent(typeof(Outline))]
public class OutlineHandler : MonoBehaviour
{
    public float showDistance = 3f;
    public LayerMask visibilityCheckMask; // ��Q���𔻒肷�郌�C���[�}�X�N�i��: Wall, Default �Ȃǁj

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

        // �J�����̒��S���炱�̃I�u�W�F�N�g�փ��C���΂�
        Vector3 dir = (transform.position - playerCamera.transform.position).normalized;
        Ray ray = new Ray(playerCamera.transform.position, dir);

        // ���C���ŏ��ɓ����������̂����̃I�u�W�F�N�g���g�ł���΁u�����Ă���v�Ɣ��f
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
