using DG.Tweening;
using NUnit.Framework.Interfaces;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class ItemShopManager : MonoBehaviour
{
    [Header("Equip Slots")]
    public ItemButtonUI helmetSlot;
    public ItemButtonUI armorSlot;
    public ItemButtonUI bootsSlot;
    public ItemButtonUI ringSlot;

    public Sprite emptySlotSprite;

    public List<ItemDataSO> helmet;
    public List<ItemDataSO> armor;
    public List<ItemDataSO> boots;
    public List<ItemDataSO> ring;

    public GameObject itemButtonPrefab;
    public Transform contentParent;  // chỗ spawn item buttons

    public Button actionButton;

    public GameObject itemInfoPanel;

    private ItemDataSO currentSelected;

    private RectTransform panelRect;

    public ItemStatUI itemStatUI;

    public TextMeshProUGUI coinText;
    public TextMeshProUGUI notificationText;



    void Awake()
    {
        if (itemInfoPanel == null)
        {
            return;
        }

        panelRect = itemInfoPanel.GetComponent<RectTransform>();
        itemInfoPanel.SetActive(false);
        LoadAllItems();
    }


    void Start()
    {
        ShowTab(helmet); // mặc định mở tab1
        RefreshEquipUI(); // cập nhật slot cố định
        OnCoinChanged(CoinManager.Instance.GetCoin());
        notificationText.text = "";


        if (CoinManager.Instance != null)
        {
            CoinManager.Instance.OnCoinChanged += OnCoinChanged;
        }
    }

    private void OnDisable()
    {
        CoinManager.Instance.OnCoinChanged -= OnCoinChanged;
    }

    private List<ItemDataSO> currentTabItems;


    public void OnCoinChanged(int newCoin)
    {
        coinText.text = newCoin.ToString();
    }

    public void ShowTab(List<ItemDataSO> tabItems)
    {
        currentTabItems = tabItems; // lưu tab hiện tại
        CloseItemInfo();
        currentSelected = null;
        currentSelectedUI = null;
        SelectItem(null, null); // reset nút action
        // clear content cũ
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        // spawn items mới
        foreach (var item in tabItems)
        {
            GameObject newBtn = Instantiate(itemButtonPrefab, contentParent);
            newBtn.transform.GetChild(1).GetComponent<Image>().sprite = item.ItemIcon;
            newBtn.transform.GetChild(4).gameObject.SetActive(!item.IsOwned); // show locked nếu chưa mua
            var itemUI = newBtn.GetComponent<ItemButtonUI>();
            itemUI.Init(item, this);
            newBtn.GetComponent<Button>().onClick.AddListener(() => SelectItem(item, itemUI));
        }
    }

    private List<ItemDataSO> GetCurrentTab()
    {
        return currentTabItems;
    }

    public void ShowHelmetTab() => ShowTab(helmet);
    public void ShowArmorTab() => ShowTab(armor);
    public void ShowBootsTab() => ShowTab(boots);
    public void ShowRingTab() => ShowTab(ring);

    private ItemButtonUI currentSelectedUI;
    public void SelectItem(ItemDataSO item, ItemButtonUI itemUI)
    {
        if(item == null || itemUI == null)
        {
            actionButton.onClick.RemoveListener(BuyItem);
            actionButton.onClick.RemoveListener(EquipItem);
            actionButton.interactable = false;
            actionButton.GetComponentInChildren<TextMeshProUGUI>().text = "Select an Item";
            return;
        }

        actionButton.interactable = true;
        // reset border cho nút trước đó (dùng currentSelectedUI cũ)
        if (currentSelectedUI != null && currentSelectedUI != itemUI)
        {
            currentSelectedUI.RefreshBorder(false);
        }

        // gán lại item hiện tại
        currentSelected = item;
        currentSelectedUI = itemUI;
        currentSelectedUI.RefreshBorder(true);

        // xử lý action button
        actionButton.onClick.RemoveListener(BuyItem);
        actionButton.onClick.RemoveListener(EquipItem);
        if (!item.IsOwned)
        {
            actionButton.GetComponentInChildren<TextMeshProUGUI>().text = "Buy " + item.ItemPrice + "G";
            actionButton.onClick.AddListener(BuyItem);
        }
        else
        {
            actionButton.GetComponentInChildren<TextMeshProUGUI>().text = item.IsEquipped ? "UnEquip" : "Equip";
            actionButton.onClick.AddListener(EquipItem);
        }
    }

    public void CloseItemInfo()
    {
        var infoUI = itemInfoPanel.GetComponent<ItemInfoPanelUI>();
        infoUI.HidePanel();
    }   
    public bool IsItemInfoShow()
    {
        return itemInfoPanel.activeSelf;
    }
    public void ShowItemInfo(ItemDataSO item, RectTransform itemButtonRect)
    {
        // set nội dung info
        var infoUI = itemInfoPanel.GetComponent<ItemInfoPanelUI>();
        infoUI.ShowInfo(item);
        infoUI.ShowPanel(itemButtonRect);
    }
  
    void BuyItem()
    {
        if (currentSelected == null) return;
        
        if (!CoinManager.Instance.TrySpend(currentSelected.ItemPrice))
        {
            ShowNotification(notificationText.text = "Not enough Gold!");
            return;
        } else
        {
            ShowNotification(currentSelected.ItemName + " Purchased!");
        }

        currentSelected.IsOwned = true;
        SelectItem(currentSelected, currentSelectedUI); // refresh info
        currentSelectedUI.SetUnlock(currentSelected.IsOwned);
    }

    void EquipItem()
    {
        if (currentSelected == null) return;

        List<ItemDataSO> currentTab = GetCurrentTab();

        if (currentSelected.IsEquipped)
        {
            currentSelected.IsEquipped = false;
            for (int i = currentEquipedItems.Count - 1; i >= 0; i--)
            {
                var itemUI = currentEquipedItems[i];
                if (itemUI != null && itemUI == currentSelected)
                {
                    currentEquipedItems.RemoveAt(i);
                    break;
                }
            }
            ShowNotification(currentSelected.ItemName + " UnEquipped!");
        }
        else
        {
            foreach (var tabItem in currentTab)
            {
                if (tabItem.IsEquipped)
                {
                    // remove corresponding itemUI 
                    for (int j = currentEquipedItems.Count - 1; j >= 0; j--)
                    {
                        var itemUI = currentEquipedItems[j];
                        if (itemUI != null && itemUI == tabItem)
                        {
                            currentEquipedItems.RemoveAt(j);
                            break;
                        }
                    }
                    tabItem.IsEquipped = false;
                }
            }

            // equip currentSelected
            currentSelected.IsEquipped = true;
            ShowNotification(currentSelected.ItemName + " Equipped!"); 
        }

        // refresh borders / slot UI
        foreach (Transform child in contentParent)
        {
            var itemUI = child.GetComponent<ItemButtonUI>();
            if (itemUI != null)
            {
                itemUI.RefreshBorder(itemUI == currentSelectedUI);
            }
        }

        RefreshEquipUI();
        actionButton.GetComponentInChildren<TextMeshProUGUI>().text = currentSelected.IsEquipped ? "UnEquip" : "Equip";

    
        itemStatUI.RefreshStatUI();
    }

    public List<ItemDataSO> currentEquipedItems = new List<ItemDataSO>();

    public void RefreshEquipUI()
    {
        SetupSlot(helmetSlot, helmet);
        SetupSlot(armorSlot, armor);
        SetupSlot(bootsSlot, boots);
        SetupSlot(ringSlot, ring);
    }


    public void ShowNotification(string notification)
    {
        notificationText.text = notification;
        CancelInvoke(nameof(ResetNotification));
        Invoke(nameof(ResetNotification), 2f);
    }

    public void ResetNotification()
    {
        notificationText.text = "";
    }

    private void SetupSlot(ItemButtonUI slot, List<ItemDataSO> tab)
    {
        var equippedItem = tab.Find(i => i.IsEquipped);
        if (equippedItem != null)
        {
            if (!currentEquipedItems.Contains(equippedItem))
            {
                currentEquipedItems.Add(equippedItem);
            }
            slot.Init(equippedItem, this);
            slot.SetIcon(equippedItem.ItemIcon);
            slot.SetUnlock(true);
            slot.gameObject.GetComponent<Button>().interactable = true;
        }
        else
        {
            if(slot.Manager == null)
            {
               slot.Init(null, this);
            }
            slot.Clear();
            slot.SetIcon(emptySlotSprite);
            slot.gameObject.GetComponent<Button>().interactable = false;
        }
    }

    // xoa tất cả dữ liệu item đã lưu (dùng để test)
    [ContextMenu("Reset Save Item")]
    public void ResetSaveItems()
    {
        List<ItemSaveData> saveList = new List<ItemSaveData>();
        foreach (var list in new List<List<ItemDataSO>> { helmet, armor, boots, ring })
        {
            foreach (var item in list)
            {
                saveList.Add(new ItemSaveData
                {
                    itemID = item.ItemID,
                    isOwned = false,
                    isEquipped = false
                });
            }
        }
        string json = JsonUtility.ToJson(new Wrapper<ItemSaveData> { items = saveList }, true);
        System.IO.File.WriteAllText(Application.persistentDataPath + "/itemData.json", json);

    }

    public void SaveAllItems()
    {
        List<ItemSaveData> saveList = new List<ItemSaveData>();
        foreach (var list in new List<List<ItemDataSO>> { helmet, armor, boots, ring })
        {
            foreach (var item in list)
            {
                saveList.Add(new ItemSaveData
                {
                    itemID = item.ItemID,
                    isOwned = item.IsOwned,
                    isEquipped = item.IsEquipped
                });
            }
        }
        string json = JsonUtility.ToJson(new Wrapper<ItemSaveData> { items = saveList }, true);
        System.IO.File.WriteAllText(Application.persistentDataPath + "/itemData.json", json);
    }

    public void LoadAllItems()
    {
        string path = Application.persistentDataPath + "/itemData.json";
        if (!System.IO.File.Exists(path)) return;

        string json = System.IO.File.ReadAllText(path);
        var wrapper = JsonUtility.FromJson<Wrapper<ItemSaveData>>(json);

        foreach (var data in wrapper.items)
        {
            foreach (var list in new List<List<ItemDataSO>> { helmet, armor, boots, ring })
            {
                foreach (var item in list)
                {
                    if (item.ItemID == data.itemID)
                    {
                        item.IsOwned = data.isOwned;
                        item.IsEquipped = data.isEquipped;
                    }
                }
            }
        }
    }

}
[System.Serializable]
public class ItemSaveData
{
    public string itemID;
    public bool isOwned;
    public bool isEquipped;
}

[System.Serializable]
public class Wrapper<T>
{
    public List<T> items;
}
