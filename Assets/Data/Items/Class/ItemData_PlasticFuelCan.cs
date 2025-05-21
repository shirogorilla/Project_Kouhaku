using UnityEngine;

[CreateAssetMenu(fileName = "PlasticFuelCan", menuName = "Items/PlasticFuelCan")]
public class ItemData_PlasticFuelCan : ItemData
{
    public int fuelAmount;               // Œ»Ý‚Ì”R—¿—Ê
    public int maxFuelAmount = 100;      // Å‘å”R—¿—e—Ê

    // Œ»Ý‚Ì”R—¿—Ê‚ðŠO•”‚©‚ç“Ç‚ÝŽæ‚è‚½‚¢ê‡
    public int CurrentAmount => fuelAmount;

    // –žƒ^ƒ“”»’è
    public bool IsFull => fuelAmount >= maxFuelAmount;

    // Š®‘S‚É–žƒ^ƒ“‚É‚·‚é
    public void Fill() => fuelAmount = maxFuelAmount;

    // ­‚µ‚¾‚¯•â[i1’PˆÊj
    public void FillUnit()
    {
        if (!IsFull)
            fuelAmount = Mathf.Min(fuelAmount + 1, maxFuelAmount);
    }

    // ”CˆÓ‚Ì—Ê‚ð•â[
    public void FillBy(int amount)
    {
        if (!IsFull)
            fuelAmount = Mathf.Min(fuelAmount + amount, maxFuelAmount);
    }

    public void ConsumeUnit()
    {
        if (CurrentAmount > 0)
        {
            fuelAmount--;
        }
    }

}
