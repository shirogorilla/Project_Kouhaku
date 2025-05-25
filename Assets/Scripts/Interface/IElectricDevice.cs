public interface IElectricDevice
{
    /// <summary>
    /// �d�����i�̌��݂̓d����ԁiON/OFF�j
    /// </summary>
    bool IsOn { get; }

    /// <summary>
    /// �d�����i�������d�́iON�̂Ƃ��̂ݎg�p�j
    /// </summary>
    float PowerConsumption { get; }

    /// <summary>
    /// �����I�ɓd����؂�i�u���[�J�[�������ɌĂ΂��j
    /// </summary>
    void ForcePowerOff();
}
