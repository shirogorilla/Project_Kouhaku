using System.Collections;
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

    [SerializeField] private float breakDelay = 5f; // 雪霊が壊すのにかかる時間（Boarded状態のみ）

    private bool isBeingAttacked = false;
    private bool isBoarding = false;
    private bool isBreak = false;
    private Coroutine boardingCoroutine;

    public WindowState CurrentState => currentState;

    private void Start()
    {
        UpdateVisuals();
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
        Debug.Log("🔧 補強開始...");

        while (timer < holdTime)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        currentState = WindowState.Boarded;
        UpdateVisuals();
        isBoarding = false;
        boardingCoroutine = null;

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
        if (currentState == WindowState.Boarded)
        {
            StartCoroutine(BreakAfterDelay(breakDelay));
        }
        else if (currentState == WindowState.Normal)
        {
            currentState = WindowState.Broken;
            isBreak = true;
            UpdateVisuals();
            Debug.Log("💥 ガラスを破壊しました");
        }
    }

    private IEnumerator BreakAfterDelay(float delay)
    {
        if (isBeingAttacked) yield break;
        isBeingAttacked = true;

        Debug.Log("⏳ 窓を破壊中...");

        yield return new WaitForSeconds(delay);

        currentState = WindowState.Broken;
        isBreak = true;
        UpdateVisuals();
        isBeingAttacked = false;

        Debug.Log("💥 窓が破壊されました（板＋ガラス）");
    }

    /// <summary>
    /// 短押しは無視（補強は長押しのみ）
    /// </summary>
    public void Interact()
    {
        // 空処理
    }
}
