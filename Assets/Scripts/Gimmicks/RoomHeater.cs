using UnityEngine;

public class RoomHeater : MonoBehaviour
{
    public bool isOn = false;
    public float heatPower = 5f; // 1�b������̉����ڈ��i��F5��/�b�j

    public virtual float GetHeatingPower()
    {
        return isOn ? heatPower : 0f;
    }
}
