using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuSceneUI : MonoBehaviour
{
    [SerializeField] private RectTransform[] safeAreaTargets;

    [Header("UI Buttons")]
    public Button map1Button;
    public Button map2Button;
    public Button map3Button;

    private void Awake()
    {
        ApplySafeArea();
    }

    private void Start()
    {
        map1Button.onClick.AddListener(() => SelectMap(GameDataManager.MapType.Map1));
        map2Button.onClick.AddListener(() => SelectMap(GameDataManager.MapType.Map2));
        map3Button.onClick.AddListener(() => SelectMap(GameDataManager.MapType.Map3));
    }

    private void SelectMap(GameDataManager.MapType mapType)
    {
        if (GameDataManager.Instance == null)
        {
            return;
        }

        GameDataManager.Instance.SetCurrentMap(mapType);

        SceneLoader.Instance.LoadScene("GameScene");
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
