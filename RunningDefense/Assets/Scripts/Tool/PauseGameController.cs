using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class PauseGameController : MonoBehaviour
{


    private TowerStats towerStats; // Tham chiếu đến TowerStats để lấy thông tin tháp
    public AbilityDps abilityDps;// Tham chiếu đến AbilityDps để lấy thông tin DPS của người chơi



    [Header("== TOWER INFO ==")]
    [SerializeField] private TextMeshProUGUI towerHpText;
    [SerializeField] private TextMeshProUGUI towerAtkPointText;
    [SerializeField] private TextMeshProUGUI towerAtkSpeedText;

    [Header("== ITEM LIST ==")]
    [SerializeField] private Transform itemListParent;
    [SerializeField] private GameObject itemSlotPrefab;

    [Header("== ANIMATION SETTINGS ==")]
    public GameObject SettingPanel;
    private CanvasGroup canvasGroup;
    private RectTransform rect;

    // Thêm biến lưu timeScale trước khi mở Pause
    private float previousTimeScale = 1f;

    private void Awake()
    {
        towerStats = GameObject.FindWithTag("Tower").GetComponent<TowerStats>();
        canvasGroup = SettingPanel.GetComponent<CanvasGroup>();
        rect = SettingPanel.GetComponent<RectTransform>();
    }

    public void Pause()
    {
        // Lưu timeScale hiện tại trước khi mở menu
        previousTimeScale = Time.timeScale;

        SettingPanel.SetActive(true);
        RefreshAllUI();

        Sequence currentSeq = DOTween.Sequence();

        // Dùng Unscaled Time để vẫn chạy khi timeScale = 0
        currentSeq.SetUpdate(true);

        canvasGroup.alpha = 0;
        rect.localScale = new Vector3(0.8f, 0.8f, 1f);

        currentSeq.Append(rect.DOScale(Vector3.one, 0.4f).SetEase(Ease.InBack));
        currentSeq.Join(canvasGroup.DOFade(1, 0.4f * 0.8f));

        currentSeq.onComplete += () =>
        {
            Time.timeScale = 0f; // Pause game sau khi anim hoàn tất
        };
    }

    public void Resume()
    {
        // Đóng panel bằng unscaled time để animation vẫn chạy dù timescale = 0
        Sequence currentSeq = DOTween.Sequence();
        currentSeq.SetUpdate(true);

        currentSeq.Append(rect.DOScale(new Vector3(0.8f, 0.8f, 1f), 0.4f).SetEase(Ease.OutBack));
        currentSeq.Join(canvasGroup.DOFade(0, 0.4f * 0.8f));

        currentSeq.onComplete += () =>
        {
            // Phục hồi timeScale về giá trị trước khi Pause() được gọi
            Time.timeScale = previousTimeScale;
            SettingPanel.SetActive(false);
        };
    }

    // Refresh toàn bộ UI

    public void RefreshAllUI()
    {
        //RefreshPlayerUI();
        RefreshTowerUI();
        RefreshItemList();
    }


    //  Tower UI
    private void RefreshTowerUI()
    {
        float towerHp = GetTowerHp();
        float towerAtk = GetTowerAtkPoint();
        float towerSpeed = GetTowerAtkSpeed();

        if (towerHpText != null)
            towerHpText.text = $"Health: {towerHp:F0}";

        if (towerAtkPointText != null)
            towerAtkPointText.text = $"Attack: {towerAtk:F1}";

        if (towerAtkSpeedText != null)
            towerAtkSpeedText.text = $"Speed Attack: {towerSpeed:F2}";
    }

    public List<GameObject> currentItemSlots = new List<GameObject>();
    public void ClearItemList()
    {
        foreach (var slot in currentItemSlots)
        {
            Destroy(slot);
        }
        currentItemSlots.Clear();
    }

    // Item List
    private void RefreshItemList()
    {
        var items = GetPlayerItems(); 
        ClearItemList();

        foreach (var item in items)
        {
            var slot = Instantiate(itemSlotPrefab, itemListParent);
            currentItemSlots.Add(slot);

            var text = slot.GetComponent<Image>();
            if (text != null)
                text.sprite = item.icon;
            var levelText = slot.GetComponentInChildren<TextMeshProUGUI>();
            if (levelText != null)
                levelText.text = $"Lv. {item.level}";
        }
    }


    private float GetTowerHp()
    {
        if (towerStats != null)
            return towerStats.baseHealth;
        else
            return 0f;
    }
    private float GetTowerAtkPoint()
    {
        if (towerStats != null)
            return towerStats.baseDamage;
        else
            return 0f;
    }
    private float GetTowerAtkSpeed()
    {
        if (towerStats != null)
            return 1 / towerStats.baseAttackSpeed;
        else
            return 0f;
    }

    private List<AbilityPauseInfo> GetPlayerItems() => abilityDps.GetSaveInfo();

    public void EndGame()
    {
        Time.timeScale = 1f; // Đảm bảo game không bị pause khi load lại
        SceneLoader.Instance.LoadScene("MenuScene");
    }
}
