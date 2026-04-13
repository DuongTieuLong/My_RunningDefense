using DG.Tweening;
using TMPro;
using UnityEngine;

public class ItemInfoPanelUI : MonoBehaviour
{
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI itemStatText;
    public TextMeshProUGUI itemDescriptionText;
    public TextMeshProUGUI itemPriceText;

    public Canvas canvas;
    public RectTransform rect;
    public CanvasGroup canvasGroup;

    public int heightControl = 640;
    public void ShowInfo(ItemDataSO item)
    {
        itemNameText.text = item.ItemName;
        itemStatText.text = GetVlue(item);
        itemDescriptionText.text = item.ItemDescription;
        itemPriceText.text = item.IsOwned ? "Owned" : $" Price: {item.ItemPrice} Gold";
    }


    private Sequence currentSeq;
    public void ShowPanel(RectTransform itemButtonRect)
    {
        // Xóa animation cũ nếu đang chạy
        if (currentSeq != null && currentSeq.IsActive())
        {
            currentSeq.Kill(true);
            currentSeq = null;
        }

        // Reset trạng thái ban đầu
        gameObject.SetActive(true);
        rect.localScale = new Vector3(0.8f, 0.8f, 1f);
        canvasGroup.alpha = 0f;

        //  Tạo sequence mới
        currentSeq = DOTween.Sequence();
        currentSeq.Append(rect.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack));
        currentSeq.Join(canvasGroup.DOFade(1f, 0.24f)); // 80% thời lượng

        // Cập nhật vị trí
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            itemButtonRect.position,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
            out localPoint
        );

        rect.anchoredPosition = localPoint + new Vector2(
            0,
            itemButtonRect.rect.height * 0.5f + rect.rect.height * 0.5f + heightControl
        );

        // 🧩 Bước 5: Giữ panel trong khung canvas
        ClampToCanvas(rect);
    }

    private void ClampToCanvas(RectTransform rect)
    {
        RectTransform canvasRect = canvas.transform as RectTransform;

        // Lấy safe area pixel (trên màn hình)
        Rect safeArea = Screen.safeArea;

        // Chuyển safe area pixel -> local trong canvas
        Vector2 anchorMin = safeArea.position;
        Vector2 anchorMax = safeArea.position + safeArea.size;
        anchorMin.x /= Screen.width;
        anchorMin.y /= Screen.height;
        anchorMax.x /= Screen.width;
        anchorMax.y /= Screen.height;

        // Tính rect safe area trong canvas
        Vector2 canvasSize = canvasRect.rect.size;
        Vector2 safeMin = new Vector2((anchorMin.x - 0.5f) * canvasSize.x, (anchorMin.y - 0.5f) * canvasSize.y);
        Vector2 safeMax = new Vector2((anchorMax.x - 0.5f) * canvasSize.x, (anchorMax.y - 0.5f) * canvasSize.y);

        // Lấy world corners của panel
        Vector3[] panelCorners = new Vector3[4];
        rect.GetWorldCorners(panelCorners);

        // Chuyển về local canvas
        for (int i = 0; i < 4; i++)
            panelCorners[i] = canvasRect.InverseTransformPoint(panelCorners[i]);

        Vector3 offset = Vector3.zero;

        // Clamp so với safe area thay vì full canvas
        if (panelCorners[0].x < safeMin.x) // left
            offset.x = safeMin.x - panelCorners[0].x + 50f;

        if (panelCorners[2].x > safeMax.x) // right
            offset.x = safeMax.x - panelCorners[2].x - 50f;

        if (panelCorners[0].y < safeMin.y) // bottom
            offset.y = safeMin.y - panelCorners[0].y + 50f;

        if (panelCorners[1].y > safeMax.y) // top
            offset.y = safeMax.y - panelCorners[1].y - 50f;

        rect.anchoredPosition += (Vector2)offset;
    }
    public void HidePanel()
    {
        if (currentSeq != null && currentSeq.IsActive())
        {
            currentSeq.Kill(true);
            currentSeq = null;
        }

        currentSeq = DOTween.Sequence();
        currentSeq.Append(canvasGroup.DOFade(0f, 0.2f));
        currentSeq.Join(rect.DOScale(0.8f, 0.2f).SetEase(Ease.InBack));
        currentSeq.OnComplete(() =>
        {
            gameObject.SetActive(false);
            currentSeq = null;
        });
    }

    public string GetVlue(ItemDataSO item)
    {
        switch (item.ItemType)
        {
            case ItemType.Helmet:
                var helmet = item as HelmetItemDataSO;
                return "Health Point: " + helmet.healthValue;
            case ItemType.Armor:
                var armor = item as ArmorItemDataSO;
                return "Health Point: " + armor.healthValue;
            case ItemType.Boots:
                var boots = item as BootsItemDataSO;
                return $"Move Speed: {boots.speedValue}\nDogdeCD: {boots.dodgeCooldownValue}s";
            case ItemType.Ring:
                var ring = item as RingItemDataSO;
                return "ATK Point: " + ring.atkValue;
            default:
                return "";
        }
    }
}
