using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;
public class SupplyShowUI : MonoBehaviour
{
    [Header("References")]
    public Image supplyIcon;     // icon hiển thị supply
    private Sequence supplySeq;
    private const float fadeTime = 0.3f;
    public void ShowSupplyIcon(Sprite icon, float duration = 2f)
    {

        // 1) Kill tween/tranfomer cũ (nếu có)
        if (supplySeq != null && supplySeq.IsActive())
        {
            supplySeq.Kill();        // dừng và huỷ sequence cũ
            supplySeq = null;
        }
        // Kill mọi tween còn tồn trên đối tượng (an toàn)
        supplyIcon.DOKill();
        supplyIcon.transform.DOKill();

        supplyIcon.sprite = icon;

        // 2) Reset trạng thái UI ngay lập tức
        supplyIcon.gameObject.SetActive(true);
        supplyIcon.color = new Color(1f, 1f, 1f, 0f); // trong suốt
        supplyIcon.transform.localScale = Vector3.zero;

        // 3) Tạo sequence mới
        supplySeq = DOTween.Sequence();
        supplySeq.Append(supplyIcon.DOFade(1f, fadeTime));
        supplySeq.Join(supplyIcon.transform.DOScale(1f, fadeTime).SetEase(Ease.OutBack));
        supplySeq.AppendInterval(duration);
        supplySeq.Append(supplyIcon.DOFade(0f, fadeTime));
        supplySeq.Join(supplyIcon.transform.DOScale(0f, fadeTime).SetEase(Ease.InBack));
        supplySeq.OnComplete(() =>
        {
            supplyIcon.gameObject.SetActive(false);
            supplySeq = null; // giải phóng ref
        });
    }
}
