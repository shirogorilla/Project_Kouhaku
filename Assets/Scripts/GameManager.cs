using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("環境設定")]
    [SerializeField] private float outsideTemperature = -10f;

    [Header("ゲーム進行設定")]
    [SerializeField] private Difficulty difficulty = Difficulty.Normal;
    [SerializeField] private int maxWaves = 5;
    [SerializeField] private float dayDuration = 300f;   // 日中のリアル秒数（例：5分）
    [SerializeField] private float nightDuration = 180f; // 夜間のリアル秒数（例：3分）

    [Header("進行状況（表示用）")]
    [SerializeField] private int currentWave = 1;
    [SerializeField] private int currentHour = 6; // 6時スタート（朝）

    private float timeAccumulator = 0f;
    private float timePerGameHour; // 難易度やWaveで決定

    // 例：通知対象（必要なら他の発電機やUIにも送れる）
    [SerializeField] private SolarGenerator solarGenerator;

    public float OutsideTemperature => outsideTemperature;
    public int CurrentHour => currentHour;
    public int CurrentWave => currentWave;
    public Difficulty CurrentDifficulty => difficulty;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        UpdateTimeSettings();
    }

    private void Update()
    {
        timeAccumulator += Time.deltaTime;
        if (timeAccumulator >= timePerGameHour)
        {
            timeAccumulator -= timePerGameHour;
            AdvanceGameHour();
        }
    }

    private void AdvanceGameHour()
    {
        currentHour = (currentHour + 1) % 24;

        Debug.Log($"⏰ ゲーム内時間: {currentHour}:00");

        solarGenerator?.OnTimeAdvanced(currentHour);

        // 24時でWave切り替え
        if (currentHour == 0)
        {
            AdvanceWave();
        }
    }

    private void AdvanceWave()
    {
        currentWave++;

        if (currentWave > maxWaves)
        {
            Debug.Log("🎉 全Wave完了！ゲームクリア！");
            // TODO: ゲーム終了処理
            return;
        }

        Debug.Log($"🌊 Wave {currentWave} 開始！");
        UpdateTimeSettings();
    }

    private void UpdateTimeSettings()
    {
        float totalDuration = dayDuration + nightDuration;
        timePerGameHour = totalDuration / 24f; // 1ゲーム時間あたりのリアル秒数

        Debug.Log($"📅 時間更新：1ゲーム時間 = {timePerGameHour:F1}秒");
    }

    public void SetOutsideTemperature(float temp)
    {
        outsideTemperature = temp;
        Debug.Log($"🌡️ 外気温を {temp}℃ に設定しました");
    }
}

public enum Difficulty
{
    Easy,
    Normal,
    Hard
}
