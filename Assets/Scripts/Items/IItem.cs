using System;

public interface IItem
{
    public void Spawn(string id);
    public void Initialize(ItemData data);
    public event Action<ItemData> DataChanged;
}

public class ItemData
{
    public ItemType ItemType;
    public string UniqId;
    public string Id;
    public float X;
    public float Y;
}

public enum ItemType
{
    Timed,
    Permanent
}
