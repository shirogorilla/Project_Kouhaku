using UnityEngine;
using TMPro;
using System.Collections;

public class TemperatureUIManager : MonoBehaviour
{
    [SerializeField] private float updateInterval = 5f;
    [SerializeField] private TextMeshProUGUI temperatureText;
    private Coroutine updateRoutine;

    private void OnEnable()
    {
        if (temperatureText == null) return;

        // ゲーム開始時に即時更新
        UpdateTemperatureImmediate();

        // 以降は定期更新
        updateRoutine = StartCoroutine(UpdateTemperatureRoutine());
    }

    private void OnDisable()
    {
        if (updateRoutine != null) StopCoroutine(updateRoutine);
    }

    private void UpdateTemperatureImmediate()
    {
        if (PlayerMovement.Instance != null)
        {
            float temp = PlayerMovement.Instance.CurrentRoomTemperature;
            float displayTemp = Mathf.Floor(temp * 10f) / 10f;
            UpdateDisplay(displayTemp);
            StartCoroutine(BlinkEffect());
        }
    }

    private IEnumerator UpdateTemperatureRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(updateInterval);

            if (PlayerMovement.Instance != null)
            {
                float temp = PlayerMovement.Instance.CurrentRoomTemperature;
                float displayTemp = Mathf.Floor(temp * 10f) / 10f;
                UpdateDisplay(displayTemp);
                StartCoroutine(BlinkEffect());
            }
            else
            {
                Debug.LogWarning("PlayerMovement.Instance が見つかりません。待機中...");
            }
        }
    }

    private void UpdateDisplay(float temp)
    {
        if (temperatureText != null)
        {
            temperatureText.text = $"{temp} °C";
        }
    }

    private IEnumerator BlinkEffect()
    {
        if (temperatureText == null) yield break;

        for (int i = 0; i < 2; i++)
        {
            temperatureText.alpha = 0f;
            yield return new WaitForSeconds(0.2f);
            temperatureText.alpha = 1f;
            yield return new WaitForSeconds(0.2f);
        }
    }
}
