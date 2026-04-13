using UnityEngine;
using System;

public class TowerUpgradeManager : MonoBehaviour
{
    public TowerStats towerStats;
    public TowerUI towerUI;

    [Header("Upgrade Settings")]
    public float commonMultiplier = 1.0f;   // Thường
    public float rareMultiplier = 1.5f;     // Hiếm
    public float epicMultiplier = 2.0f;     // Cực hiếm

    public enum UpgradeType { Health, AttackSpeed, Damage }
    public enum Rarity { Common, Rare, Epic }

    private System.Random rng = new System.Random();

    void Awake()
    {
        if (!towerStats) towerStats = GetComponent<TowerStats>();
        if (!towerUI) towerUI = GetComponent<TowerUI>();
    }

    void OnEnable() => towerStats.OnLevelUp += ShowUpgradeChoices;
    void OnDisable() => towerStats.OnLevelUp -= ShowUpgradeChoices;

    /// <summary>
    /// Gọi khi TowerStats thông báo lên level
    /// </summary>
    void ShowUpgradeChoices(int level)
    {
        // Tạo ba lựa chọn: Máu, Tốc độ bắn, Damage
        TowerUpgradeOption[] options = new TowerUpgradeOption[3];
        options[0] = GenerateOption(UpgradeType.Health);
        options[1] = GenerateOption(UpgradeType.AttackSpeed);
        options[2] = GenerateOption(UpgradeType.Damage);

        // Gửi dữ liệu cho TowerUI để hiển thị popup
        towerUI.ShowUpgradePopup(options, ApplyUpgrade);
    }

    TowerUpgradeOption GenerateOption(UpgradeType type)
    {
        // Random độ hiếm
        int rarityRoll = rng.Next(0, 100);
        Rarity rarity;
        if (rarityRoll < 60) rarity = Rarity.Common;
        else if (rarityRoll < 90) rarity = Rarity.Rare;
        else rarity = Rarity.Epic;

        // Xác định giá trị cộng
        float baseValue = 0;
        switch (type)
        {
            case UpgradeType.Health: baseValue = 10f; break;
            case UpgradeType.AttackSpeed: baseValue = -0.05f; break; // tốc bắn giảm cooldown
            case UpgradeType.Damage: baseValue = 5f; break;
        }

        float multiplier = rarity == Rarity.Common ? commonMultiplier
                          : rarity == Rarity.Rare ? rareMultiplier
                          : epicMultiplier;

        float value = baseValue * multiplier;

        return new TowerUpgradeOption(type, rarity, value);
    }

    void ApplyUpgrade(TowerUpgradeOption choice)
    {
        switch (choice.type)
        {
            case UpgradeType.Health:
                towerStats.baseHealth += choice.value;
                break;
            case UpgradeType.AttackSpeed:
                towerStats.baseAttackSpeed = Mathf.Max(0.1f, towerStats.baseAttackSpeed + choice.value);
                break;
            case UpgradeType.Damage:
                towerStats.baseDamage += choice.value;
                break;
        }
        towerStats.ApplyStats();
    }
}

/// <summary>
/// Dữ liệu một lựa chọn nâng cấp
/// </summary>
[System.Serializable]
public struct TowerUpgradeOption
{
    public TowerUpgradeManager.UpgradeType type;
    public TowerUpgradeManager.Rarity rarity;
    public float value;

    public TowerUpgradeOption(TowerUpgradeManager.UpgradeType type, TowerUpgradeManager.Rarity rarity, float value)
    {
        this.type = type;
        this.rarity = rarity;
        this.value = value;
    }
}
