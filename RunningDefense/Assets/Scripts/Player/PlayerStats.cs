using Cysharp.Threading.Tasks;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    public BasePlayerStats baseStats;
    public AbilityDps abilityDps;
    public UpgradeUI upgradeUI;


    public event Action OnDeath;

    [Header("Player Stats")]
    public float maxHealth = 100;
    public float currentHealth;
    public float atkPoint = 20;
    public float dogdeCooldown = 2f; // thời gian hồi dodge
    public float moveSpeed = 5f;
    [SerializeField] private bool dodgeTime = false;  

    public AudioClip hitSFX;
    public bool DodgeTime
    {
        get { return dodgeTime; }
        set { dodgeTime = value; }
    }

    [Header("EXP / Level")]
    public int level = 1;
    public float currentExp = 0f;
    public float expToLevelUp = 50f;
    public float expGrowth = 1.5f;

    public event Action OnLevelUp;

    public HealthBarBillboard healthBar;
    public Slider expBar;

    public TextMeshProUGUI levelText;
    public TextMeshProUGUI dpsText;

    private Animator animator;

    public  bool IsDead { get; private set; } = false;

    private void OnEnable()
    {
        // Đăng ký lắng nghe sự kiện EnemyKilled
        Enemy.OnEnemyKilled += HandleEnemyKilled;
        upgradeUI.OnUpgradeSelected += UpdateLevelUI;
    }

    private void OnDisable()
    {
        // Hủy đăng ký để tránh memory leak
        Enemy.OnEnemyKilled -= HandleEnemyKilled;
        upgradeUI.OnUpgradeSelected -= UpdateLevelUI;
    }

    private void Awake()
    {
        var statsObj = GameObject.FindWithTag("BasePlayerStats");
        if (statsObj != null)
            baseStats = GameObject.FindWithTag("BasePlayerStats").GetComponent<BasePlayerStats>();
    }
    private void Start()
    {
        maxHealth = baseStats.MaxHP;
        atkPoint = baseStats.ATK;
        moveSpeed = baseStats.MoveSpeed;
        dogdeCooldown = baseStats.DodgeCooldown;

        currentHealth = maxHealth;

        animator = GetComponent<Animator>();
    }

    public void TakeDamge(float damage)
    {
        if (dodgeTime) return; // Trong thời gian dodge thì bỏ qua sát thương
        SoundManager.Instance.PlaySFX(hitSFX);  
        currentHealth -= damage;
        healthBar.UpdateHealthBar(currentHealth / maxHealth);
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
    }

    public void Heal(float hp)
    {
        currentHealth += hp;
        if(currentHealth >= maxHealth)
        {
            currentHealth = maxHealth;
        }
        healthBar.UpdateHealthBar(currentHealth / maxHealth);
    }

    public void RespawnHealth()
    {
        var m = GetComponent<PlayerMove>();
        m.enabled = true;
        m.rb.isKinematic = false;
        IsDead = false;
        currentHealth = maxHealth;
        healthBar.UpdateHealthBar(currentHealth / maxHealth);
    }
    public void Die()
    {
        if (!IsDead)
        {
            var m = GetComponent<PlayerMove>();
            m.rb.isKinematic = true;
            m.enabled = false;
            animator.SetTrigger("Die");
            IsDead = true;
            OnDeath?.Invoke();
        }
    }

    public void UpdateLevelUI()
    {
        levelText.text = $"Level {level}";
        float dps = abilityDps.GetTotalDPS();
        dpsText.text = $"DPS: {dps:F1}";
    }

    /// <summary>
    /// Hàm xử lý EXP khi enemy bị tiêu diệt
    /// </summary>
    private void HandleEnemyKilled(GameObject killer, float exp)
    {
        if (killer == this.gameObject)
        {
            GainExp(exp);
        }
    }

    private void GainExp(float exp)
    {
        currentExp += exp;
        expBar.value = currentExp / expToLevelUp;

        if (currentExp >= expToLevelUp)
        {
            LevelUp();
        }
    }

    private void LevelUp()
    {
        level++;
        currentExp -= expToLevelUp;
        expToLevelUp *= expGrowth;
        expBar.value = currentExp / expToLevelUp;

        // Tăng chỉ số khi lên level (có thể điều chỉnh theo game design)
        maxHealth *= 1.1f;
        atkPoint *= 1.15f;
        currentHealth = maxHealth;

        OnLevelUp?.Invoke();
    }

    // NOTE: Phần liên quan tới tấn công
    // - Khi bạn viết script tấn công (ví dụ PlayerAttack, PlayerBullet),
    //   hãy đảm bảo khi gây sát thương gọi:
    //   enemy.TakeDamage(damage, this.gameObject);
    //   để enemy biết "attacker" là player này.
    // - Nếu không truyền attacker, EXP sẽ không được cộng.
}
