using TMPro;
using UnityEngine;

public class FuelGeneratorUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI fuelText;
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private FuelGenerator generator;

    private void Update()
    {
        if (generator == null) return;

        fuelText.text = $" {generator.GetFuelAmount()} / {generator.GetMaxFuel()}";
        statusText.text = generator.IsRunning() ? "‰Ò“­’†" : "’âŽ~’†";
    }
}
