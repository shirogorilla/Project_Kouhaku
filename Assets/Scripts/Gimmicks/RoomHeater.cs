using UnityEngine;

public class RoomHeater : MonoBehaviour
{
    public bool isOn = false;
    public float heatPower = 5f; // 1秒あたりの加温目安（例：5℃/秒）

    public virtual float GetHeatingPower()
    {
        return isOn ? heatPower : 0f;
    }
}
