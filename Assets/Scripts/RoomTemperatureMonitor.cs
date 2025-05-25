using UnityEngine;
using TMPro;
using System.Text;

public class RoomTemperatureMonitor : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textDisplay;
    [SerializeField] private RoomTemperature[] rooms;

    private void Update()
    {
        if (textDisplay == null || rooms == null || rooms.Length == 0) return;

        StringBuilder sb = new StringBuilder();

        // 外気温（1つ目のRoomTemperatureから取得）
        float outsideTemp = rooms[0].OutsideTemperature;
        sb.AppendLine($"Outside: {outsideTemp:F1}°C");
        sb.AppendLine("-----");

        // 各部屋の温度
        foreach (var room in rooms)
        {
            sb.AppendLine($"{room.name}: {room.temperature:F1}°C");
        }

        textDisplay.text = sb.ToString();
    }
}
