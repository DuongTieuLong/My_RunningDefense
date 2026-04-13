using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(CanvasGroup))]
public class UIAnimator : MonoBehaviour
{
    public enum AnimationType
    {
        Fade,
        Scale,
        Slide,
        FadeAndScale
    }

    public enum SlideDirection
    {
        Left,
        Right,
        Up,
        Down
    }

    public enum IdleType
    {
        None,
        Pulse,      // co giãn nhịp nhàng
        Float,      // trôi lên xuống
        Shake,      // rung nhẹ
        Rotate,     // xoay qua lại
        Wiggle      // lắc nhỏ
    }

    [Header("🎞️ Toggle Animation")]
    public bool useToggleAnimation = true;
    public AnimationType animationType = AnimationType.FadeAndScale;
    public SlideDirection slideDirection = SlideDirection.Up;
    public float duration = 0.4f;
    public Ease easeIn = Ease.OutBack;
    public Ease easeOut = Ease.InBack;

    [Header("🎨 Customization")]
    public float fadeFrom = 0f;
    public float fadeTo = 1f;
    public Vector3 scaleFrom = new Vector3(0.8f, 0.8f, 1f);
    public Vector3 scaleTo = Vector3.one;
    public float slideDistance = 200f;

    [Header("🧩 Options")]
    public bool startHidden = true;
    public bool deactivateOnClose = true;

    [Header("✨ Idle Animation (loop trong Update)")]
    public IdleType idleType = IdleType.None;
    public float idleAmplitude = 10f; // biên độ
    public float idleSpeed = 1f;      // tốc độ
    public bool idleAffectRotation = false;
    public bool idleAffectScale = true;

    private CanvasGroup canvasGroup;
    private RectTransform rect;
    private Vector2 originalPos;
    private Vector3 originalScale;
    private Quaternion originalRot;
    private bool isOpen = false;

    private Sequence currentSeq;
    private float idleTime;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        rect = GetComponent<RectTransform>();
        originalPos = rect.anchoredPosition;
        originalScale = rect.localScale;
        originalRot = rect.localRotation;

        if (startHidden && useToggleAnimation)
            gameObject.SetActive(false);
    }

    void Update()
    {
        // Idle Loop — chỉ chạy nếu active
        if (idleType != IdleType.None && gameObject.activeInHierarchy)
        {
            idleTime += Time.deltaTime * idleSpeed;
            ApplyIdleAnimation();
        }
    }

    private void ApplyIdleAnimation()
    {
        switch (idleType)
        {
            case IdleType.Pulse:
                if (idleAffectScale)
                {
                    float s = 1 + Mathf.Sin(idleTime * Mathf.PI * 2) * (idleAmplitude * 0.01f);
                    rect.localScale = originalScale * s;
                }
                break;

            case IdleType.Float:
                rect.anchoredPosition = originalPos + Vector2.up * Mathf.Sin(idleTime * Mathf.PI * 2) * idleAmplitude;
                break;

            case IdleType.Shake:
                rect.anchoredPosition = originalPos + new Vector2(
                    Mathf.PerlinNoise(idleTime * 2, 0) - 0.5f,
                    Mathf.PerlinNoise(0, idleTime * 2) - 0.5f
                ) * idleAmplitude;
                break;

            case IdleType.Rotate:
                if (idleAffectRotation)
                {
                    float angle = Mathf.Sin(idleTime * Mathf.PI * 2) * idleAmplitude;
                    rect.localRotation = Quaternion.Euler(0, 0, angle);
                }
                break;

            case IdleType.Wiggle:
                if (idleAffectRotation)
                {
                    float smallAngle = Mathf.Sin(idleTime * Mathf.PI * 2) * idleAmplitude * 0.3f;
                    rect.localRotation = Quaternion.Euler(0, 0, smallAngle);
                }
                if (idleAffectScale)
                {
                    float wiggleScale = 1 + Mathf.Sin(idleTime * Mathf.PI * 4) * (idleAmplitude * 0.005f);
                    rect.localScale = originalScale * wiggleScale;
                }
                break;
        }
    }

    // === TOGGLE ===
    public void ToggleUI()
    {
        if (!useToggleAnimation) return;

        if (isOpen) CloseUI();
        else OpenUI();
    }
    public void ToggleUIIngoreTimeScale()
    {
        if (!useToggleAnimation) return;
        if (isOpen) CloseUI(true);
        else OpenUI(true);
    }

    public void OpenUI(bool ignoreTimeScale = false)
    {
        if (!useToggleAnimation) return;
        if (isOpen) return;

        isOpen = true;
        gameObject.SetActive(true);
        currentSeq?.Kill();
        currentSeq = DOTween.Sequence();
        if (ignoreTimeScale)
            currentSeq.SetUpdate(true);


        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();


        switch (animationType)
        {
            case AnimationType.Fade:
                rect.localScale = scaleTo;
                canvasGroup.alpha = fadeFrom;
                currentSeq.Append(canvasGroup.DOFade(fadeTo, duration));
                break;

            case AnimationType.Scale:
                canvasGroup.alpha = fadeTo;
                rect.localScale = scaleFrom;
                currentSeq.Append(rect.DOScale(scaleTo, duration).SetEase(easeIn));
                break;

            case AnimationType.Slide:
                rect.localScale = scaleTo;
                canvasGroup.alpha = fadeTo;
                rect.anchoredPosition = GetSlideFromPosition();
                currentSeq.Append(rect.DOAnchorPos(originalPos, duration).SetEase(easeIn));
                break;

            case AnimationType.FadeAndScale:
                canvasGroup.alpha = fadeFrom;
                rect.localScale = scaleFrom;
                currentSeq.Append(rect.DOScale(scaleTo, duration).SetEase(easeIn));
                currentSeq.Join(canvasGroup.DOFade(fadeTo, duration * 0.8f));
                break;
        }

        currentSeq.OnComplete(() => canvasGroup.blocksRaycasts = true);
    }

    public void CloseUI(bool ignoreTimeScale = false)
    {
        if (!useToggleAnimation) return;
        if (!isOpen) return;

        isOpen = false;
        canvasGroup.blocksRaycasts = false;
        currentSeq?.Kill();
        currentSeq = DOTween.Sequence();
        if (ignoreTimeScale)
            currentSeq.SetUpdate(true);

        switch (animationType)
        {
            case AnimationType.Fade:
                currentSeq.Append(canvasGroup.DOFade(fadeFrom, duration));
                break;
            case AnimationType.Scale:
                currentSeq.Append(rect.DOScale(scaleFrom, duration).SetEase(easeOut))
                    .OnComplete(() => canvasGroup.alpha = fadeFrom);
                break;
            case AnimationType.Slide:
                currentSeq.Append(rect.DOAnchorPos(GetSlideFromPosition(), duration).SetEase(easeOut))
                    .OnComplete(() =>
                    {
                        canvasGroup.alpha = fadeFrom;
                        rect.anchoredPosition = originalPos;
                    });
                break;
            case AnimationType.FadeAndScale:
                currentSeq.Append(rect.DOScale(scaleFrom, duration).SetEase(easeOut));
                currentSeq.Join(canvasGroup.DOFade(fadeFrom, duration * 0.8f));
                break;
        }

        currentSeq.OnComplete(() =>
        {
            if (deactivateOnClose)
                gameObject.SetActive(false);
        });
    }

    private Vector2 GetSlideFromPosition()
    {
        Vector2 offset = Vector2.zero;
        switch (slideDirection)
        {
            case SlideDirection.Left: offset = Vector2.left * slideDistance; break;
            case SlideDirection.Right: offset = Vector2.right * slideDistance; break;
            case SlideDirection.Up: offset = Vector2.up * slideDistance; break;
            case SlideDirection.Down: offset = Vector2.down * slideDistance; break;
        }
        return originalPos + offset;
    }
}
