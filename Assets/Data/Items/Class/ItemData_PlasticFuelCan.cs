using UnityEngine;

[CreateAssetMenu(fileName = "PlasticFuelCan", menuName = "Items/PlasticFuelCan")]
public class ItemData_PlasticFuelCan : ItemData
{
    public int fuelAmount;               // 現在の燃料量

    // 現在の燃料量を外部から読み取りたい場合
    public int CurrentAmount => fuelAmount;

    // 満タン判定
    public bool IsFull => fuelAmount >= maxFuelAmount;

    // 完全に満タンにする
    public void Fill() => fuelAmount = maxFuelAmount;

    // 少しだけ補充（1単位）
    public void FillUnit()
    {
        if (!IsFull)
            fuelAmount = Mathf.Min(fuelAmount + 1, maxFuelAmount);
    }

    // 任意の量を補充
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
