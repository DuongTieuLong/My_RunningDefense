using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Unity.VisualScripting;

public class ItemButtonUI : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
{
    public Image borderImage;   // ảnh viền
    public Image borderEquiped;

    public GameObject lockedObject;

    private ItemDataSO itemData;
    public ItemShopManager manager;

    public ItemShopManager Manager => manager;

    private float pointerDownTime;
    private bool isPointerDown;

    public float holdThreshold = 0.5f; // giữ 0.5s thì gọi là "giữ"

    public bool inspectClickMode = false;

    private void Update()
    {
        if(!inspectClickMode)
        {
            if (isPointerDown && itemData != null && manager != null)
            {
                float heldTime = Time.time - pointerDownTime;
                if (heldTime >= holdThreshold)
                {
                    manager.ShowItemInfo(itemData, GetComponent<RectTransform>());
                    isPointerDown = false; // reset để không spam liên tục
                }
            }
        } else
        {
            if (isPointerDown && itemData != null && manager != null)
            {
              manager.ShowItemInfo(itemData, GetComponent<RectTransform>());
            }
        }
       
    }

    public void Init(ItemDataSO data, ItemShopManager invManager)
    {
        itemData = data;
        manager = invManager;
        RefreshBorder(false); // mặc định không chọn
    }


    public void RefreshBorder(bool isSelected)
    {

            //borderImage.sprite = isSelected ? manager.borderSelected : manager.borderDefault;

        // Equipped border (overlay riêng)
        if (itemData != null && borderEquiped != null)
        {
            borderEquiped.enabled = itemData.IsEquipped;
        }
    }

    public ItemDataSO GetItemData() => itemData;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isPointerDown)
        {
            if (manager.IsItemInfoShow() && itemData != null && !inspectClickMode)
            {
                manager.ShowItemInfo(itemData, GetComponent<RectTransform>());
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isPointerDown = true;
        pointerDownTime = Time.time;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPointerDown = false;
    }

    public void SetUnlock(bool locked)
    {
        if (lockedObject != null)
            lockedObject.SetActive(!locked);
    }

    public void SetIcon(Sprite icon)
    {
        transform.GetChild(1).GetComponent<Image>().sprite = icon;
    }

    public void Clear()
    {
        itemData = null;
        RefreshBorder(false);
    }
}

