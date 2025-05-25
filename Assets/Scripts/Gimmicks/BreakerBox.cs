using UnityEngine;

public class BreakerBox : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        if (PowerManager.Instance == null) return;

        if (PowerManager.Instance.IsBreakerTripped())
        {
            PowerManager.Instance.ResetBreaker();
            Debug.Log("🔌 ブレーカーが復旧されました");
        }
        else
        {
            Debug.Log("✅ ブレーカーは正常です（復旧不要）");
        }
    }

    public void CancelInteract() { }
}
