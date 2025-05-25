using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PowerStatusUI : MonoBehaviour
{
    [Header("UI参照")]
    [SerializeField] private TextMeshProUGUI maxPowerText;
    [SerializeField] private TextMeshProUGUI currentUsageText;
    [SerializeField] private TextMeshProUGUI breakerStatusText;

    [Header("更新間隔(秒)")]
    [SerializeField] private float updateInterval = 0.5f;

    private float timer;

    private void Update()
    {
        if (PowerManager.Instance == null) return;

        timer += Time.deltaTime;
        if (timer >= updateInterval)
        {
            timer = 0f;
            UpdateDisplay();
        }
    }

    private void UpdateDisplay()
    {
        float maxPower = PowerManager.Instance.GetMaxCapacity();
        float currentUsage = PowerManager.Instance.GetCurrentUsage();
        bool isTripped = PowerManager.Instance.IsBreakerTripped();

        maxPowerText.text = $"最大電力: {maxPower:F0} W";
        currentUsageText.text = $"使用電力: {currentUsage:F0} W";

        if (isTripped)
        {
            breakerStatusText.text = "⚠️ ブレーカーが落ちました！";
            breakerStatusText.color = Color.red;
        }
        else
        {
            breakerStatusText.text = "✅ 電力は正常です";
            breakerStatusText.color = Color.green;
        }
    }
}
