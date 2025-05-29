using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerStatusUI : MonoBehaviour
{
    [Header("HP")]
    [SerializeField] private TextMeshProUGUI hpText;

    [Header("Bars")]
    [SerializeField] private Slider nutritionBar;
    [SerializeField] private Slider fatigueBar;
    [SerializeField] private Slider temperatureBar;

    [SerializeField] private PlayerStatus playerStatus;

    private void Start()
    {
        if (playerStatus != null)
        {
            playerStatus.OnStatusChanged += UpdateUI;
            UpdateUI();
        }
    }

    private void UpdateUI()
    {
        hpText.text = $" {(int)playerStatus.CurrentHP}";

        nutritionBar.value = playerStatus.Nutrition / playerStatus.MaxValue;
        fatigueBar.value = playerStatus.Fatigue / playerStatus.MaxValue;
        temperatureBar.value = playerStatus.BodyTemperature / playerStatus.MaxValue;
    }
}
