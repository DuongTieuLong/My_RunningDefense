using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameSceneUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Button backButton;
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private TMP_Text fpsText;

    [SerializeField] private RectTransform[] safeAreaTargets;
    private float elapsedTime;

    // FPS tracking
    private float fpsUpdateInterval = 0.5f; // cập nhật mỗi 0.5s
    private float fpsTimeLeft;
    private float fpsAccum; // tổng thời gian unscaled
    private int fpsFrames;  // số frame trong interval

    private void Awake()
    {
        ApplySafeArea();

        if (backButton != null)
            backButton.onClick.AddListener(OnBackButton);

        fpsTimeLeft = fpsUpdateInterval;
        fpsAccum = 0f;
        fpsFrames = 0;
    }

    private void Update()
    {
        UpdateTimer();
        UpdateFPS();
    }

    private void UpdateTimer()
    {
        // Cập nhật thời gian tăng dần (bị ảnh hưởng bởi Time.timeScale)
        elapsedTime += Time.deltaTime;

        int minutes = Mathf.FloorToInt(elapsedTime / 60f);
        int seconds = Mathf.FloorToInt(elapsedTime % 60f);

        if (timerText != null)
            timerText.text = $"{minutes:00}:{seconds:00}";
    }

    private void UpdateFPS()
    {
        // --- FPS (dùng unscaled để vẫn đo được khi Time.timeScale = 0) ---
        fpsTimeLeft -= Time.unscaledDeltaTime;
        fpsAccum += Time.unscaledDeltaTime;
        fpsFrames++;

        if (fpsTimeLeft <= 0f)
        {
            float fps = fpsFrames / Mathf.Max(0.0001f, fpsAccum);
            if (fpsText != null)
                fpsText.text = $"{fps:F1} FPS";

            // reset cho interval tiếp theo
            fpsTimeLeft = fpsUpdateInterval;
            fpsAccum = 0f;
            fpsFrames = 0;
        }
    }

    private void OnBackButton()
    {
       // SceneManager.LoadScene("MenuScene");
    }

    void ApplySafeArea()
    {
        Rect safeArea = Screen.safeArea;
        Vector2 minAnchor = safeArea.position;
        Vector2 maxAnchor = safeArea.position + safeArea.size;

        minAnchor.x /= Screen.width;
        minAnchor.y /= Screen.height;
        maxAnchor.x /= Screen.width;
        maxAnchor.y /= Screen.height;

        foreach (RectTransform rect in safeAreaTargets)
        {
            rect.anchorMin = minAnchor;
            rect.anchorMax = maxAnchor;
        }
    }
    
}
