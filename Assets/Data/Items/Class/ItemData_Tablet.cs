using UnityEngine;

[CreateAssetMenu(fileName = "Tablet", menuName = "Items/Tablet")]
public class ItemData_Tablet : ItemData
{
    [Header("タブレット専用プロパティ")]
    public Sprite tabletUIScreen; // UI表示用イメージ（サムネイルなど）

    public bool canAccessMap = true;
    public bool canAccessLogs = true;
    public bool canAccessSystem = true;

    // 他のタブレット専用機能があればここに追加
}