using UnityEngine;
using System;
using TMPro;

[RequireComponent(typeof(TowerHealth))]
[RequireComponent(typeof(TowerAttack))]
public class TowerStats : MonoBehaviour
{
    [Header("Base Stats")]
    public float baseHealth = 100f;
    public float baseDamage = 10f;
    public float baseAttackSpeed = 1f; // số giây giữa mỗi đòn bắn
    public float baseAttackRadius = 5f;

    [Header("Level & EXP")]
    public int level = 1;
    public float currentExp = 0f;
    public float expToLevelUp = 50f; // exp cần để lên level đầu tiên
    public float expGrowth = 1.5f;   // mỗi lần lên level tăng yêu cầu EXP
    public TextMeshProUGUI levelText;

    public event Action<int> OnLevelUp;

    private TowerHealth health;
    private TowerAttack attack;

    void OnEnable() => Enemy.OnEnemyKilled += HandleEnemyKilled;
    void OnDisable() => Enemy.OnEnemyKilled -= HandleEnemyKilled;
    public event Action<float, float> OnExpChanged;

    void HandleEnemyKilled(GameObject killer, float exp)
    {
        if (killer == this.gameObject) // chính tower
        {
            GainExp(exp);
        }
    }

    void Awake()
    {
        health = GetComponent<TowerHealth>();
        attack = GetComponent<TowerAttack>();
    }

    void Start()
    {
        ApplyStats();
    }

    /// <summary>Gọi khi nhận EXP từ Enemy</summary>
    public void GainExp(float exp)
    {
        currentExp += exp;
        OnExpChanged?.Invoke(currentExp, expToLevelUp);

        if (currentExp >= expToLevelUp)
        {
            LevelUp();
        }
    }

    void LevelUp()
    {
        level++;
        currentExp -= expToLevelUp;
        expToLevelUp *= expGrowth;

        ApplyStats();
        OnLevelUp?.Invoke(level);

        // Cập nhật UI ngay sau khi lên level
        OnExpChanged?.Invoke(currentExp, expToLevelUp);
        levelText.text = $"Level {level}";
    }

    public void ApplyStats()
    {
        // Cập nhật máu
        health.SetMaxHealth(baseHealth);

        // Cập nhật chỉ số tấn công
        attack.attackDamage = Mathf.RoundToInt(baseDamage);
        attack.attackCooldown = baseAttackSpeed;
        attack.attackRadius = baseAttackRadius;
    }
}
