using UnityEngine;

public enum ItemType
{
    Usable,         // �g�p�\�A�C�e��
    Placeable,      // �ݒu�\�A�C�e��
    Bag,            // �i�b�v�T�b�N(�A�C�e���X���b�g�g���p)
    PlasticFuelCan, // �|���^���N(�R�������^�їp)
    WoodenPlank,    // ��(���̕⋭�p)
    KeyItem,        // ���A�C�e���i����g���p�j
}

[CreateAssetMenu(fileName = "New Item", menuName = "Items/Item")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public string description;
    public int maxStackAmount = 10;  // �ő及����
    public ItemType itemType = ItemType.Usable;

    public GameObject placeablePrefab;

    public int maxFuelAmount;
}
