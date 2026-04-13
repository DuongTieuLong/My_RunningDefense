using UnityEngine;

public abstract class ItemDataSO : ScriptableObject, IItemData
{
    [SerializeField] private string itemID;
    [SerializeField] private ItemType itemType;
    [SerializeField] private string itemName;
    [SerializeField] private Sprite itemIcon;
    [SerializeField] private int itemPrice;
    [SerializeField] private string itemDescription;
    [SerializeField] private bool isOwned = false;
    [SerializeField] private bool isEquipped = false;

    public string ItemID {  get => itemID; set => itemID = value; }
    public ItemType ItemType { get => itemType; set => itemType = value; }
    public string ItemName { get => itemName; set => itemName = value; }
    public Sprite ItemIcon { get => itemIcon; set => itemIcon = value; }
    public int ItemPrice { get => itemPrice; set => itemPrice = value; }
    public string ItemDescription { get => itemDescription; set => itemDescription = value; }
    public bool IsOwned { get => isOwned; set => isOwned = value; }
    public bool IsEquipped { get => isEquipped; set => isEquipped = value; }
}
