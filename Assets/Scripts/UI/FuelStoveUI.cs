using TMPro;
using UnityEngine;

public class FuelStoveUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI fuelText;
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private FuelStove stove;

    private void Update()
    {
        if (stove == null) return;

        fuelText.text = $" {stove.GetFuelAmount()} / {stove.GetMaxFuel()}";
        statusText.text = stove.IsRunning() ? "‰Ò“­’†" : "’âŽ~’†";
    }
}