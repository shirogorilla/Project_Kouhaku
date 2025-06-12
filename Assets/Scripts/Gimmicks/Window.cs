using System.Collections;
using UnityEngine.UI;
using UnityEngine;

public enum WindowState
{
    Normal,     // ガラスはあるが補強なし（無防備）
    Boarded,    // 板で補強済み（防衛状態）
    Broken      // ガラスも板も壊れて侵入可能
}

public class Window : MonoBehaviour, IInteractable
{
    [SerializeField] private WindowState currentState = WindowState.Normal;
    [SerializeField] private RoomTemperature parentRoom;

    [SerializeField] private GameObject glassObject;   // ガラス表示オブジェクト
    [SerializeField] private GameObject boardObject;   // 補強板の表示オブジェクト

    [SerializeField] private float breakDelayNormal = 5f; // 通常状態を壊すのにかかる時間
    [SerializeField] private float breakDelayBoarded = 15f; // 補強状態を壊すのにかかる時間

    private bool isBeingAttacked = false;
    private bool isBoarding = false;
    private bool isBreak = false;
    private Coroutine boardingCoroutine;
    private Coroutine breakingCoroutine;

    public WindowState CurrentState => currentState;

    [SerializeField] private GameObject durabilityGaugeRoot;
    [SerializeField] private UnityEngine.UI.Slider durabilitySlider;
    private Image durabilityFillImage;

    [SerializeField] private GameObject boardGaugeRoot;
    [SerializeField] private UnityEngine.UI.Slider boardGaugeSlider;

    private void Start()
    {
        UpdateVisuals();
        HideDurabilityGauge();

        if (durabilitySlider != null && durabilitySlider.fillRect != null)
        {
            durabilityGaugeRoot.gameObject.SetActive(false);
            durabilityFillImage = durabilitySlider.fillRect.GetComponent<Image>();
        }
    }

    /// <summary>
    /// 状態に応じて表示の切り替え
    /// </summary>
    private void UpdateVisuals()
    {
        if (glassObject != null)
            glassObject.SetActive(isBreak != true); // ガラスは破壊されている場合、非表示

        if (boardObject != null)
            boardObject.SetActive(currentState == WindowState.Boarded); // 板はBoardedのときだけ表示

        // 🌡️ 温度管理に通知（Broken時のみ冷却）
        if (parentRoom != null)
        {
            if (currentState == WindowState.Broken)
            {
                parentRoom.AddOpenWindow();
            }
            else
            {
                parentRoom.RemoveOpenWindow();
            }
        }
    }

    /// <summary>
    /// 窓を補強する（外部から長押しで開始）
    /// </summary>
    public void StartBoardingExternally(ItemData_WoodenPlank plank)
    {
        if ((currentState != WindowState.Normal && currentState != WindowState.Broken) || isBoarding)
        {
            Debug.Log("⚠️ 補強できる状態ではありません");
            return;
        }

        boardingCoroutine = StartCoroutine(BoardingRoutine(plank));
    }

    private IEnumerator BoardingRoutine(ItemData_WoodenPlank plank)
    {
        float holdTime = (currentState == WindowState.Broken) ? 3.5f : 2.5f;
        float timer = 0f;

        isBoarding = true;
        boardGaugeRoot.SetActive(true);
        boardGaugeSlider.value = 0f;

        Debug.Log("🔧 補強開始...");

        while (timer < holdTime)
        {
            timer += Time.deltaTime;

            if (boardGaugeSlider != null)
            {
                boardGaugeSlider.value = timer / holdTime;  // スライダー更新
            }

            yield return null;
        }

        currentState = WindowState.Boarded;
        UpdateVisuals();
        isBoarding = false;
        boardingCoroutine = null;

        boardGaugeRoot.SetActive(false);
        boardGaugeSlider.value = 0f;

        // 板アイテムを1つ消費
        InventoryManager.Instance?.ConsumeItem(plank);

        Debug.Log("🛡️ 補強完了！");
    }

    /// <summary>
    /// 補強中のキャンセル処理（長押し中断）
    /// </summary>
    public void CancelInteract()
    {
        if (isBoarding && boardingCoroutine != null)
        {
            StopCoroutine(boardingCoroutine);
            boardingCoroutine = null;
            isBoarding = false;

            boardGaugeRoot.SetActive(false);
            boardGaugeSlider.value = 0f;

            Debug.Log("❌ 補強キャンセルされました");
        }
    }

    /// <summary>
    /// 雪霊がこの窓から侵入可能かどうか
    /// </summary>
    public bool IsPassable()
    {
        return currentState == WindowState.Broken;
    }

    /// <summary>
    /// 窓を破壊（雪霊側が使う）
    /// </summary>
    public void BreakWindow()
    {
        if (isBeingAttacked) return;

        float delay = (currentState == WindowState.Boarded) ? breakDelayBoarded : breakDelayNormal;
        breakingCoroutine = StartCoroutine(BreakAfterDelay(delay));
    }

    private IEnumerator BreakAfterDelay(float delay)
    {
        isBeingAttacked = true;

        Debug.Log("⏳ 窓を破壊中...");

        ShowDurabilityGauge();
        float timer = 0f;

        while (timer < delay)
        {
            timer += Time.deltaTime;

            // 耐久ゲージ更新
            UpdateDurabilityGauge(timer / delay);

            yield return null;
        }

        currentState = WindowState.Broken;
        isBreak = true;
        UpdateVisuals();
        isBeingAttacked = false;
        breakingCoroutine = null;

        HideDurabilityGauge();

        Debug.Log("💥 窓が破壊されました");
    }

    private void ShowDurabilityGauge()
    {
        if (durabilityGaugeRoot != null)
            durabilityGaugeRoot.SetActive(true);

        if (durabilitySlider != null)
            durabilitySlider.value = 1f;
    }

    private void HideDurabilityGauge()
    {
        if (durabilityGaugeRoot != null)
            durabilityGaugeRoot.SetActive(false);
    }

    private void UpdateDurabilityGauge(float progress)
    {
        if (durabilitySlider != null)
        {
            float value = Mathf.Clamp01(1f - progress);
            durabilitySlider.value = value;

            if (durabilityFillImage != null)
            {
                // 緑→黄→赤 に色変化
                Color green = Color.green;
                Color yellow = Color.yellow;
                Color red = Color.red;

                if (value > 0.5f)
                {
                    // 0.5～1 は 緑→黄 に線形補間
                    float t = (value - 0.5f) * 2f; // 0～1
                    durabilityFillImage.color = Color.Lerp(yellow, green, t);
                }
                else
                {
                    // 0～0.5 は 黄→赤 に線形補間
                    float t = value * 2f; // 0～1
                    durabilityFillImage.color = Color.Lerp(red, yellow, t);
                }
            }
        }
    }

    /// <summary>
    /// 短押しは無視（補強は長押しのみ）
    /// </summary>
    public void Interact()
    {
        // 空処理
    }
}
