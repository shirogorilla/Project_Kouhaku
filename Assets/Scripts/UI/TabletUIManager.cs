using UnityEngine;
using UnityEngine.LowLevel;

public class TabletUIManager : MonoBehaviour
{
    public static TabletUIManager Instance;

    [SerializeField] private GameObject tabletUIRoot;
    [SerializeField] private PlayerMovement playerMovement;
    private bool isOpen = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (tabletUIRoot != null)
            tabletUIRoot.SetActive(false);
    }

    public void OpenTablet()
    {
        if (isOpen) return;

        tabletUIRoot.SetActive(true);
        isOpen = true;

        // 時間停止
        Time.timeScale = 0f;

        // カーソルロック解除
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // プレイヤー操作(視点操作)を止める
        if (playerMovement != null)
            playerMovement.enabled = false;
    }

    public void CloseTablet()
    {
        if (!isOpen) return;

        tabletUIRoot.SetActive(false);
        isOpen = false;

        // 時間再開
        Time.timeScale = 1f;

        // カーソルロック
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // プレイヤー操作(視点操作)再開
        if (playerMovement != null)
            playerMovement.enabled = true;
    }
}
