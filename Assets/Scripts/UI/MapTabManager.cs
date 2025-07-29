using UnityEngine;
using UnityEngine.UI;

public class MapTabManager : MonoBehaviour
{
    [Header("階層タブのボタン")]
    [SerializeField] private Button firstFloorButton;
    [SerializeField] private Button secondFloorButton;

    [Header("フロアマップ")]
    [SerializeField] private GameObject firstFloorMap;
    [SerializeField] private GameObject secondFloorMap;

    private void Start()
    {
        // 最初に1Fを表示（初期状態）
        ShowFirstFloor();

        // ボタンにリスナー登録
        firstFloorButton.onClick.AddListener(ShowFirstFloor);
        secondFloorButton.onClick.AddListener(ShowSecondFloor);
    }

    private void ShowFirstFloor()
    {
        firstFloorMap.SetActive(true);
        secondFloorMap.SetActive(false);
    }

    private void ShowSecondFloor()
    {
        firstFloorMap.SetActive(false);
        secondFloorMap.SetActive(true);
    }
}
