using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class LoadingGamePanel : MonoBehaviour
{
    public static LoadingGamePanel Instance;

    [Header("UI References")]
    public Slider loadingSlider;
    public TextMeshProUGUI loadingText;
    public GameObject loadingPanel;

    [Header("Fake Loading Settings")]
    public float fakeLoadTime = 3f; // thời gian giả lập (giây)
    


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        StartLoading();
    }


    public void StartLoading()
    {
        StartCoroutine(FakeLoading());
    }
    private IEnumerator FakeLoading()
    {
        loadingPanel.SetActive(true);

        float elapsed = 0f;
        loadingSlider.value = 0f;

        while (elapsed < fakeLoadTime)
        {
            elapsed += Time.unscaledDeltaTime;
            float progress = Mathf.Clamp01(elapsed / fakeLoadTime);

            if(progress < 0.9f)
            {
                progress = progress / 0.9f * 0.9f; 
            }
            else
            {
                progress = 0.9f + (progress - 0.9f) / 0.1f * 0.1f; 
            }

            loadingSlider.value = progress;
            loadingText.text = Mathf.RoundToInt(progress * 100f) + "%";

            yield return null;
        }

        loadingSlider.value = 1f;
        loadingText.text = "100%";
        yield return new WaitForSecondsRealtime(0.5f); // đợi một chút trước khi ẩn panel
        LoadingGamePanel.Instance.loadingPanel.SetActive(false);
    }
}
