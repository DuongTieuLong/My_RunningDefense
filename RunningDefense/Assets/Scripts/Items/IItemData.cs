using UnityEngine;

public interface IItemData
{
    public string ItemID { get; set; }
    public ItemType ItemType { get; set; }
    public string ItemName { get; set; }
    public Sprite ItemIcon { get; set; }
    public int ItemPrice { get; set; }
    public string ItemDescription { get; set; }
    public bool IsOwned { get; set; }
    public bool IsEquipped { get; set; }

}

public enum ItemType
{
    Helmet,
    Armor,
    Boots,
    Ring,
}
