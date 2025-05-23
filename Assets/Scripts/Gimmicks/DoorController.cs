using System.Collections;
using UnityEngine;

public class DoorController : MonoBehaviour, IInteractable
{
    [SerializeField] private float openAngle = 90f;
    [SerializeField] private float openSpeed = 2f;
    [SerializeField] private float forceOpenSpeed = 10f; // 敵専用の高速開閉スピード

    [Header("温度拡散")]
    public RoomTemperature roomA;
    public RoomTemperature roomB;
    public float diffusionRate = 0.05f;

    private Quaternion closedRotation;
    private Quaternion openRotation;
    private bool isOpen = false;
    public bool IsOpen => isOpen;
    private Coroutine currentAnim;

    private void Start()
    {
        closedRotation = transform.rotation;
        openRotation = Quaternion.Euler(0, transform.eulerAngles.y + openAngle, 0);
    }

    public void Interact()
    {
        ToggleDoor();
    }

    public void CancelInteract()
    {
        // 今は何もしない
    }

    private void ToggleDoor()
    {
        isOpen = !isOpen;
        if (currentAnim != null) StopCoroutine(currentAnim);
        currentAnim = StartCoroutine(RotateDoor(isOpen ? openRotation : closedRotation, openSpeed));
    }

    private IEnumerator RotateDoor(Quaternion targetRot, float speed)
    {
        while (Quaternion.Angle(transform.rotation, targetRot) > 0.1f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * speed);
            yield return null;
        }
        transform.rotation = targetRot;
    }

    public void ForceOpen(Vector3 fromPosition)
    {
        if (isOpen) return;

        isOpen = !isOpen;
        Vector3 dir = (transform.position - fromPosition).normalized;
        float dot = Vector3.Dot(transform.right, dir);

        float angle = dot > 0 ? -openAngle : openAngle;
        Quaternion targetRotation = Quaternion.Euler(0f, angle, 0f) * closedRotation;

        if (currentAnim != null) StopCoroutine(currentAnim);
        currentAnim = StartCoroutine(RotateDoor(targetRotation, forceOpenSpeed));
    }

    public RoomTemperature GetOtherRoom(RoomTemperature current)
    {
        return current == roomA ? roomB : current == roomB ? roomA : null;
    }
}
