public interface IElectricDevice
{
    /// <summary>
    /// 電化製品の現在の電源状態（ON/OFF）
    /// </summary>
    bool IsOn { get; }

    /// <summary>
    /// 電化製品が消費する電力（ONのときのみ使用）
    /// </summary>
    float PowerConsumption { get; }

    /// <summary>
    /// 強制的に電源を切る（ブレーカー落下時に呼ばれる）
    /// </summary>
    void ForcePowerOff();
}
