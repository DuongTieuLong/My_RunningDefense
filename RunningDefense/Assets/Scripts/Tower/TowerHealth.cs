using UnityEngine;
using System;

public class TowerHealth : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100;

    public float MaxHealth => maxHealth;
    public float currentHealth { get; private set; }

    public bool IsDead { get; private set; }

    // Sự kiện thay đổi máu
    public event Action<float, float> OnHealthChanged;

    private void Start()
    {
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void Regeneration(float health)
    {
        currentHealth = Mathf.Min(currentHealth + health, maxHealth);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0) Die();
    }

    public void SetMaxHealth(float newMax)
    {
        // Nếu đang full máu, tăng luôn currentHealth lên max mới
        bool wasFull = Mathf.Approximately(currentHealth, maxHealth);
        maxHealth = newMax;
        currentHealth = wasFull ? maxHealth : Mathf.Min(currentHealth, maxHealth);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    private void Die()
    {
        if (!IsDead)
        {
            GameSessionManager.Instance.EndGame();
            Destroy(gameObject);
            IsDead = true;
        }
    }
}
