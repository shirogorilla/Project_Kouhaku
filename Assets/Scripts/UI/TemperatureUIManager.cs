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
        updateRoutine = StartCoroutine(UpdateTemperatureRoutine());
    }

    private void OnDisable()
    {
        if (updateRoutine != null) StopCoroutine(updateRoutine);
    }

    private IEnumerator UpdateTemperatureRoutine()
    {
        while (true)
        {
            if (PlayerMovement.Instance != null)
            {
                float temp = PlayerMovement.Instance.CurrentRoomTemperature;

                // 小数点以下1桁で切り捨て
                float displayTemp = Mathf.Floor(temp * 10f) / 10f;

                UpdateDisplay(displayTemp);
                StartCoroutine(BlinkEffect());
            }
            else
            {
                Debug.LogWarning("PlayerMovement.Instance が見つかりません。待機中...");
            }

            yield return new WaitForSeconds(updateInterval);
        }
    }

    private void UpdateDisplay(float temp)
    {
        if (temperatureText != null)
        {
            temperatureText.text = $"{temp} °C"; // フォントに ° が含まれていればOK
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
