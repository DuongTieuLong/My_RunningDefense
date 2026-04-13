using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class TowerIndicator : MonoBehaviour
{
    [SerializeField] private Transform tower;
    [SerializeField] private Transform player;
    [SerializeField] private RectTransform indicatorUI;
    [SerializeField] private TextMeshProUGUI distanceText;
    [SerializeField] private Canvas parentCanvas; // ⚠️ Gán Canvas chứa indicator

    private CanvasGroup cg;
    private Tween fadeTween;
    private bool isVisible = false;

    private Camera cam;

    private void Awake()
    {
        cg = indicatorUI.GetComponent<CanvasGroup>();
        cam = parentCanvas.worldCamera; // Lấy camera từ canvas
        DOTween.SetTweensCapacity(1000, 100);
    }

    private void Update()
    {
        if (tower == null || player == null || indicatorUI == null || distanceText == null || cam == null)
            return;

        Vector3 viewportPos = cam.WorldToViewportPoint(tower.position);

        bool isOffScreen = viewportPos.z < 0 ||
                           viewportPos.x < 0 || viewportPos.x > 1 ||
                           viewportPos.y < 0 || viewportPos.y > 1;

        // Fade theo trạng thái
        if (isOffScreen && !isVisible)
        {
            FadeIndicator(true);
            isVisible = true;
        }
        else if (!isOffScreen && isVisible)
        {
            FadeIndicator(false);
            isVisible = false;
        }

        if (!isOffScreen)
            return;

        // Clamp viền UI (0.1f–0.9f cho thẩm mỹ)
        viewportPos.x = Mathf.Clamp(viewportPos.x, 0.1f, 0.9f);
        viewportPos.y = Mathf.Clamp(viewportPos.y, 0.1f, 0.9f);

        Vector3 screenPos = cam.ViewportToScreenPoint(viewportPos);

        // ⚠️ Chuyển đổi sang toạ độ local trong canvas
        RectTransform canvasRect = parentCanvas.GetComponent<RectTransform>();
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPos, cam, out Vector2 localPos))
        {
            indicatorUI.localPosition = localPos;
        }

        // Xoay hướng icon
        Vector3 dir = (tower.position - player.position).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Vector3 e = indicatorUI.localEulerAngles;
        e.z = angle + 90f;
        indicatorUI.localEulerAngles = e;

        // Cập nhật khoảng cách
        float distance = Vector3.Distance(player.position, tower.position);
        distanceText.text = $"{distance:F0}m";
    }

    private void FadeIndicator(bool show)
    {
        fadeTween?.Kill();
        fadeTween = cg.DOFade(show ? 1f : 0f, 0.5f)
                         .SetUpdate(true)
                         .SetEase(Ease.OutQuad);
    }
}
