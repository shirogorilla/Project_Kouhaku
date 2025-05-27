using TMPro;
using UnityEngine;

public class GameTimeUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private RectTransform cycleUI;
    [SerializeField] private float blinkInterval = 0.75f;
    [SerializeField] private float rotateSpeed = 2.5f; // 回転のなめらかさ（大きいほど速く追従）

    private float blinkTimer = 0f;
    private bool colonVisible = true;

    private Quaternion targetRotation;

    private void Start()
    {
        // 初期回転を現在時刻に合わせて設定
        if (GameManager.Instance != null)
        {
            int hour = GameManager.Instance.CurrentHour;
            SetTargetRotation(hour);
            cycleUI.localRotation = targetRotation;
        }
    }

    private void Update()
    {
        if (GameManager.Instance == null) return;

        int hour = GameManager.Instance.CurrentHour;

        // ⏰ 点滅テキスト処理
        blinkTimer += Time.deltaTime;
        if (blinkTimer >= blinkInterval)
        {
            blinkTimer = 0f;
            colonVisible = !colonVisible;
        }
        timeText.text = FormatHour(hour, colonVisible);

        // 🔄 UI回転処理
        SetTargetRotation(hour); // 時間に対応した目標角度を更新
        cycleUI.localRotation = Quaternion.Lerp(cycleUI.localRotation, targetRotation, Time.deltaTime * rotateSpeed);
    }

    private string FormatHour(int hour, bool showColon)
    {
        return showColon ? $"{hour:00}:00" : $"{hour:00} 00";
    }

    private void SetTargetRotation(int hour)
    {
        float angle = (hour % 24) * 15f; // 1時間＝15度
        targetRotation = Quaternion.Euler(0f, 0f, -angle); // 時計回りにするためマイナス
    }
}
