using UnityEngine;

public abstract class ElectricAppliance : MonoBehaviour, IElectricDevice
{
    [SerializeField] protected float powerConsumption = 50f;
    public float PowerConsumption => IsOn ? powerConsumption : 0f;

    public bool IsOn { get; protected set; } = false;

    public virtual void OnPowerOn() { }
    public virtual void OnPowerOff() { }

    protected virtual void Start()
    {
        // 自動でPowerManagerに登録（必要に応じて手動化も可能）
        PowerManager.Instance?.RegisterDevice(this);
    }

    protected virtual void OnDestroy()
    {
        PowerManager.Instance?.UnregisterDevice(this);
    }

    /// <summary>
    /// 外部呼び出し用：電源ONリクエスト(ブレーカー落ち確認アリ)
    /// </summary>
    public virtual void TryTurnOn()
    {
        if (PowerManager.Instance == null) return;

        if (PowerManager.Instance.CanConsumePower(powerConsumption))
        {
            IsOn = true;
            PowerManager.Instance.ReportPowerUsage(powerConsumption); // 使用量加算
            OnPowerOn();
            Debug.Log($"{gameObject.name} が ON になりました");
        }
        else
        {
            Debug.LogWarning($"{gameObject.name} をONにできません（ブレーカー落ち or 容量オーバー）");
        }
    }

    /// <summary>
    /// 外部呼び出し用：電源ONリクエスト(ブレーカー落ち確認ナシ)
    /// </summary>
    public virtual void TurnOn()
    {
        if (PowerManager.Instance == null) return;
        if (PowerManager.Instance.IsBreakerTripped()) return; // ブレーカーが落ちている場合、ONできない

        IsOn = true;
        PowerManager.Instance.ReportPowerUsage(powerConsumption); // 使用量加算
        OnPowerOn();
        Debug.Log($"{gameObject.name} が ON になりました");
    }

    /// <summary>
    /// 外部呼び出し用：電源OFF
    /// </summary>
    public virtual void TurnOff()
    {
        IsOn = false;
        PowerManager.Instance.ReportPowerUsage(-powerConsumption); // 使用量減算
        OnPowerOff();
        Debug.Log($"{gameObject.name} が OFF になりました");
    }

    /// <summary>
    /// ブレーカー落下などによる強制OFF
    /// </summary>
    public virtual void ForcePowerOff()
    {
        IsOn = false;
        PowerManager.Instance.ReportPowerUsage(-powerConsumption); // 使用量減算
        OnPowerOff();
        Debug.LogWarning($"{gameObject.name} はブレーカー落下により強制OFFされました");
    }
}
