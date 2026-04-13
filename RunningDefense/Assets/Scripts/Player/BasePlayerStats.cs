using System;
using System.Buffers.Text;
using System.Collections.Generic;
using UnityEngine;

public class BasePlayerStats : MonoBehaviour
{
    public static BasePlayerStats Instance;
    [Header("Player Stats")]
    public float maxHealth = 100;
    public float currentHealth;
    public float atkPoint = 20;
    public float dogdeCooldown = 2f; // thời gian hồi dodge
    public float moveSpeed = 5f;
    [SerializeField] private bool dodgeTime = false;
    public bool DodgeTime
    {
        get { return dodgeTime; }
        set { dodgeTime = value; }
    }



    // Modifiers
    private List<float> hpModifiers = new List<float>();
    private List<float> atkModifiers = new List<float>();
    private List<float> moveSpeedModifiers = new List<float>();
    private List<float> dodgeCDModifiers = new List<float>();


    public float MaxHP
    {
        get
        {
            float total = maxHealth;
            foreach (float mod in hpModifiers) total += mod;
            return total;
        }
    }

    public float ATK
    {
        get
        {
            float total = atkPoint;
            foreach (float mod in atkModifiers) total += mod;
            return total;
        }
    }

    public float MoveSpeed
    {
        get
        {
            float total = moveSpeed;
            foreach (float mod in moveSpeedModifiers) total += mod;
            return total;
        }
    }

    public float DodgeCooldown
    {
        get
        {
            float total = dogdeCooldown;
            foreach (float mod in dodgeCDModifiers) total += mod;
            return total;
        }
    }

    // Các hàm apply/remove modifier
    public void AddHPBonus(float value)
    {
        hpModifiers.Add(value);
        currentHealth = Mathf.Min(currentHealth, MaxHP);
    }

    public void AddATKBonus(float value) => atkModifiers.Add(value);

    public void AddMoveSpeedBonus(float value) => moveSpeedModifiers.Add(value);

    public void AddDodgeCDBonus(float value) => dodgeCDModifiers.Add(value);

    public void ClearItemModifiers()
    {
        hpModifiers.Clear();
        atkModifiers.Clear();
        moveSpeedModifiers.Clear();
        dodgeCDModifiers.Clear();
    }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

}
