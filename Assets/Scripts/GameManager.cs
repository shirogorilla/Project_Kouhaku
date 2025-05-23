using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private float outsideTemperature = -10f; // 初期値（例）

    public float OutsideTemperature => outsideTemperature;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void SetOutsideTemperature(float temp)
    {
        outsideTemperature = temp;
        Debug.Log($"🌡️ 外気温を {temp}℃ に設定しました");
    }

    // 例：Waveや時間帯ごとの温度変化をここで処理してもOK
}
