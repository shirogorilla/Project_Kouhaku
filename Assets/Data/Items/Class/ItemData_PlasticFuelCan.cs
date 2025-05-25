using UnityEngine;

[CreateAssetMenu(fileName = "PlasticFuelCan", menuName = "Items/PlasticFuelCan")]
public class ItemData_PlasticFuelCan : ItemData
{
    public int fuelAmount;               // ���݂̔R����
    public int maxFuelAmount = 100;      // �ő�R���e��

    // ���݂̔R���ʂ��O������ǂݎ�肽���ꍇ
    public int CurrentAmount => fuelAmount;

    // ���^������
    public bool IsFull => fuelAmount >= maxFuelAmount;

    // ���S�ɖ��^���ɂ���
    public void Fill() => fuelAmount = maxFuelAmount;

    // ����������[�i1�P�ʁj
    public void FillUnit()
    {
        if (!IsFull)
            fuelAmount = Mathf.Min(fuelAmount + 1, maxFuelAmount);
    }

    // �C�ӂ̗ʂ��[
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
