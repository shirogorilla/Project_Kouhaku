public interface IPlaceableItem
{
    ItemData ItemData { get; }
    void OnLongPressRemove();
}
