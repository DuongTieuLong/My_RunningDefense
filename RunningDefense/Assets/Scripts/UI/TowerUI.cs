using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TowerUI : MonoBehaviour
{
    [Header("References")]
    public TowerHealth towerHealth;
    public TowerStats towerStats;
    public Slider healthBar;
    public Slider expBar;

    [Header("Upgrade Popup References")]
    public GameObject upgradePopup;
    public Button[] upgradeButtons;          // 3 Button
    public Image[] rarityBackgrounds;        // 3 Background Image để tô màu
    public TMP_Text[] valueTexts;            // 3 Text để hiển thị giá trị cộng

    public Sprite backCommon;
    public Sprite backRare;
    public Sprite backEpic;

    // Gọi từ TowerUpgradeManager khi tower lên level
    public void ShowUpgradePopup(TowerUpgradeOption[] options, System.Action<TowerUpgradeOption> onSelected)
    {
        upgradePopup.SetActive(true);
        // pause game while popup shown
        Time.timeScale = 0f;

        int count = Mathf.Min(options.Length, upgradeButtons.Length);
        for (int i = 0; i < upgradeButtons.Length; i++)
        {
            upgradeButtons[i].gameObject.SetActive(i < count);

            if (i < count)
            {
                var opt = options[i];

                // nhãn loại nâng cấp
                string label = opt.type == TowerUpgradeManager.UpgradeType.Health ? "Health"
                                     : opt.type == TowerUpgradeManager.UpgradeType.AttackSpeed ? "Attack\nSpeed"
                                     : "Damage";

                // format giá trị: AttackSpeed hiển thị là +X% (ví dụ -0.05 => +5%)
                string valueStr;
                if (opt.type == TowerUpgradeManager.UpgradeType.AttackSpeed)
                {
                    float percent = (opt.value < 0f) ? -opt.value * 100f : opt.value * 100f;
                    valueStr = $"+{percent:F0}%";
                }
                else
                {
                    valueStr = opt.value >= 0 ? $"+{opt.value:F0}" : $"{opt.value:F0}";
                }

                valueTexts[i].text = $"{label}\n{valueStr}";

                // set rarity background / button listeners ...
                // Thay đổi background sprite theo độ hiếm
                SetBackgroundRare(rarityBackgrounds[i], opt.rarity);

                int idx = i;
                upgradeButtons[i].onClick.RemoveAllListeners();
                upgradeButtons[i].GetComponent<ButtonSound>().ReAddListener();
                upgradeButtons[i].onClick.AddListener(() =>
                {
                    onSelected?.Invoke(opt);
                    CloseUpgradePopup();
                });
            }
        }
    }

    // đóng popup và resume time
    public void CloseUpgradePopup()
    {
        if (upgradePopup != null)
            upgradePopup.SetActive(false);
        Time.timeScale = 1f;
    }

    private void OnDestroy()
    {
        // đảm bảo resume khi object bị hủy
        Time.timeScale = 1f;
    }

    // Thêm hàm mới này và xóa hàm GetRarityColor cũ
    private void SetBackgroundRare(Image targetBackground, TowerUpgradeManager.Rarity rarity)
    {
        switch (rarity)
        {
            case TowerUpgradeManager.Rarity.Common:
                targetBackground.sprite = backCommon;
                break;
            case TowerUpgradeManager.Rarity.Rare:
                targetBackground.sprite = backRare;
                break;
            case TowerUpgradeManager.Rarity.Epic:
                targetBackground.sprite = backEpic;
                break;
            default:
                targetBackground.sprite = backCommon;
                break;
        }
    }

    void Awake()
    {
        if (!towerHealth) towerHealth = GetComponent<TowerHealth>();
        if (!towerStats) towerStats = GetComponent<TowerStats>();
    }

    void OnEnable()
    {
        if (towerHealth != null) towerHealth.OnHealthChanged += UpdateHealthBar;
        if (towerStats != null) towerStats.OnExpChanged += UpdateExpBar;
    }

    void OnDisable()
    {
        if (towerHealth != null) towerHealth.OnHealthChanged -= UpdateHealthBar;
        if (towerStats != null) towerStats.OnExpChanged -= UpdateExpBar;
    }

    void Start()
    {
        if (towerHealth) UpdateHealthBar(towerHealth.currentHealth, 100);
        if (towerStats) UpdateExpBar(towerStats.currentExp, towerStats.expToLevelUp);
    }

    void UpdateHealthBar(float current, float max)
    {
        if (healthBar != null)
            healthBar.value = current / max;
    }

    void UpdateExpBar(float current, float required)
    {
        if (expBar != null)
            expBar.value = current / required;
    }
}
