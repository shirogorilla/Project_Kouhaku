using UnityEngine;
using System;

public class PlayerStatus : MonoBehaviour
{
    public event Action OnStatusChanged;

    public float MaxHP = 100f;
    public float MaxValue = 100f; // ���ʍő�l�i�h�{�E��J�E�̉��j

    public float CurrentHP { get; private set; }
    public float Nutrition { get; private set; }
    public float Fatigue { get; private set; }
    public float BodyTemperature { get; private set; }

    private void Awake()
    {
        CurrentHP = MaxHP;
        Nutrition = Fatigue = BodyTemperature = MaxValue;
    }

    private void Update()
    {
        // ���̌������W�b�N�i��Œ����\�j
        Nutrition = Mathf.Max(0, Nutrition - Time.deltaTime * 0.1f);
        Fatigue = Mathf.Max(0, Fatigue - Time.deltaTime * 0.05f);
        BodyTemperature = Mathf.Max(0, BodyTemperature - Time.deltaTime * 0.07f);

        // ��ԕω���HP�ɂ��e�����o���
        if (Nutrition <= 0 || Fatigue <= 0 || BodyTemperature <= 0)
        {
            CurrentHP = Mathf.Max(0, CurrentHP - Time.deltaTime * 0.5f);
        }

        OnStatusChanged?.Invoke();
    }

    public void Heal(float amount)
    {
        CurrentHP = Mathf.Min(CurrentHP + amount, MaxHP);
        OnStatusChanged?.Invoke();
    }

    public void Eat(float amount)
    {
        Nutrition = Mathf.Min(Nutrition + amount, MaxValue);
        OnStatusChanged?.Invoke();
    }

    public void Rest(float amount)
    {
        Fatigue = Mathf.Min(Fatigue + amount, MaxValue);
        OnStatusChanged?.Invoke();
    }

    public void WarmUp(float amount)
    {
        BodyTemperature = Mathf.Min(BodyTemperature + amount, MaxValue);
        OnStatusChanged?.Invoke();
    }
}
