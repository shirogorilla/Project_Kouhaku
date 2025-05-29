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
        // ������PowerManager�ɓo�^�i�K�v�ɉ����Ď蓮�����\�j
        PowerManager.Instance?.RegisterDevice(this);
    }

    protected virtual void OnDestroy()
    {
        PowerManager.Instance?.UnregisterDevice(this);
    }

    /// <summary>
    /// �O���Ăяo���p�F�d��ON���N�G�X�g(�u���[�J�[�����m�F�A��)
    /// </summary>
    public virtual void TryTurnOn()
    {
        if (PowerManager.Instance == null) return;

        if (PowerManager.Instance.CanConsumePower(powerConsumption))
        {
            IsOn = true;
            PowerManager.Instance.ReportPowerUsage(powerConsumption); // �g�p�ʉ��Z
            OnPowerOn();
            Debug.Log($"{gameObject.name} �� ON �ɂȂ�܂���");
        }
        else
        {
            Debug.LogWarning($"{gameObject.name} ��ON�ɂł��܂���i�u���[�J�[���� or �e�ʃI�[�o�[�j");
        }
    }

    /// <summary>
    /// �O���Ăяo���p�F�d��ON���N�G�X�g(�u���[�J�[�����m�F�i�V)
    /// </summary>
    public virtual void TurnOn()
    {
        if (PowerManager.Instance == null) return;
        if (PowerManager.Instance.IsBreakerTripped()) return; // �u���[�J�[�������Ă���ꍇ�AON�ł��Ȃ�

        IsOn = true;
        PowerManager.Instance.ReportPowerUsage(powerConsumption); // �g�p�ʉ��Z
        OnPowerOn();
        Debug.Log($"{gameObject.name} �� ON �ɂȂ�܂���");
    }

    /// <summary>
    /// �O���Ăяo���p�F�d��OFF
    /// </summary>
    public virtual void TurnOff()
    {
        IsOn = false;
        PowerManager.Instance.ReportPowerUsage(-powerConsumption); // �g�p�ʌ��Z
        OnPowerOff();
        Debug.Log($"{gameObject.name} �� OFF �ɂȂ�܂���");
    }

    /// <summary>
    /// �u���[�J�[�����Ȃǂɂ�鋭��OFF
    /// </summary>
    public virtual void ForcePowerOff()
    {
        IsOn = false;
        PowerManager.Instance.ReportPowerUsage(-powerConsumption); // �g�p�ʌ��Z
        OnPowerOff();
        Debug.LogWarning($"{gameObject.name} �̓u���[�J�[�����ɂ�苭��OFF����܂���");
    }
}
