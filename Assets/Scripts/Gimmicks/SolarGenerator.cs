using UnityEngine;

public class SolarGenerator : MonoBehaviour, IInteractable
{
    private int storedStacks = 0;

    [SerializeField] private float powerIncrease = 300f;

    private bool isNightMode = false;
    private bool isActivated = false;

    // ゲーム時間進行から呼び出される（1時間に1回）
    public void OnTimeAdvanced(int currentHour)
    {
        // 6～18時を昼とする（2時間ごとに蓄電）
        if (currentHour >= 6 && currentHour < 18)
        {
            if (currentHour % 2 == 0)
            {
                storedStacks++;
                Debug.Log($"🔆 太陽光発電: 蓄電 +1 → 合計 {storedStacks} スタック");
            }
        }
        else if (isNightMode && isActivated)
        {
            // 夜間で稼働中ならスタック消費
            storedStacks--;
            Debug.Log($"🌙 時間経過でスタック消費: -1 → 残り {storedStacks}");

            if (storedStacks <= 0)
            {
                Debug.Log("🌙 スタック切れ：夜間稼働OFF");
                TurnOff();
            }
        }
    }

    public void Interact()
    {
        if (!isNightMode) return;

        if (isActivated)
        {
            TurnOff();
        }
        else
        {
            if (storedStacks > 0)
            {
                TurnOn();
            }
            else
            {
                Debug.Log("🔋 蓄電が空です。使用できません。");
            }
        }
    }

    private void TurnOn()
    {
        isActivated = true;
        PowerManager.Instance.AddPowerCapacity(powerIncrease);
        Debug.Log("🌙 太陽光発電: 夜間稼働 ON");
    }

    private void TurnOff()
    {
        isActivated = false;
        PowerManager.Instance.RemovePowerCapacity(powerIncrease);
        Debug.Log("🌙 太陽光発電: 夜間稼働 OFF");
    }

    public void StartNightMode()
    {
        isNightMode = true;
        Debug.Log("🌙 夜モード：手動使用が可能になりました");
    }

    public void EndNightMode()
    {
        isNightMode = false;
        TurnOff();
    }

    public void CarryOverToNextWave()
    {
        Debug.Log($"📦 Waveまたぎ：未使用スタック {storedStacks} を持ち越し");
    }

    public void CancelInteract()
    {
        Debug.Log("稼働 一時停止");
        TurnOff();
    }

    public int GetStoredStacks() => storedStacks;
    public bool IsActive() => isActivated;
}
