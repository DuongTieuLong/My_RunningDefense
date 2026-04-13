using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : MonoBehaviour
{
    [Header("Weapon Settings")]
    public Transform player;
    public PlayerStats playerStats;
    public string abilityName = "Sword";
    public Sprite icon;
    [SerializeField] private int abilityLevel = 1;
    [SerializeField] private float rotateSpeed;
    [SerializeField] private float damage;
    [SerializeField] private float radius = 2f;
    [SerializeField] private float height = 1f;
    public AudioClip sfx;
    public ParticleSystem hitEffect;

    [Header("Blade")]
    public Transform bladeStart;
    public Transform bladeEnd;
    public float bladeRadius = 0.1f;

    [Header("Debug")]
    public bool debugGizmos = true;

    private float currentAngle = 0f;
    private HashSet<Enemy> damagedEnemies = new HashSet<Enemy>();

    GameObject playerOwner;
    private void Start()
    {
        playerOwner = GameObject.FindGameObjectWithTag("Player");
        playerStats = player.GetComponent<PlayerStats>();
    }

    void Update()
    {
        if (player == null) return;

        float prevAngle = currentAngle;
        currentAngle += rotateSpeed * Time.deltaTime;

        // reset danh sách khi xoay xong 1 vòng
        int prevRot = Mathf.FloorToInt(prevAngle / 180f);
        int newRot = Mathf.FloorToInt(currentAngle / 180f);

        if (newRot > prevRot)
        {
            damagedEnemies.Clear();
        }

        // giữ góc trong [0,360)
        currentAngle = Mathf.Repeat(currentAngle, 360f);

        // --- orbit quanh player ---
        Vector3 offset = new Vector3(
            Mathf.Cos(currentAngle * Mathf.Deg2Rad) * radius,
            height,
            Mathf.Sin(currentAngle * Mathf.Deg2Rad) * radius
        );

        transform.position = player.position + offset;

        Vector3 outwardDir = (transform.position - (player.position + new Vector3(0,height,0))).normalized;
        Quaternion lookRot = Quaternion.LookRotation(outwardDir, Vector3.down);
        transform.rotation = lookRot;
        transform.rotation = lookRot * Quaternion.Euler(90f, 0f, 0f);

        EnemiesTrigger();   
    }

    List<Enemy> toDamage = new List<Enemy>();
    void EnemiesTrigger()
    {
        if (bladeStart == null || bladeEnd == null) return;
        if (EnemyManager.Instance == null) return;

        List<Enemy> enemies = EnemyManager.Instance.ActiveEnemies;
        foreach (var enemy in enemies)
        {
            if (enemy == null || enemy.isDead) continue;

            if (IsEnemyInBladeRange(enemy) && damagedEnemies.Add(enemy))
                toDamage.Add(enemy);
        }
        foreach (var enemy in toDamage)
        {
            float finalDamage = damage + playerStats.atkPoint;
            SoundManager.Instance.PlaySFX(sfx);
            hitEffect.Play();
            enemy.TakeDamage(finalDamage, playerOwner);
        }
        toDamage.Clear();
    }

    bool IsEnemyInBladeRange(Enemy enemy)
    {
        Vector3 start = bladeStart.position;
        Vector3 end = bladeEnd.position;
        Vector3 enemyPos = enemy.transform.position;

        // khoảng cách từ điểm tới đoạn line (start–end)
        Vector3 lineDir = end - start;
        float lineLength = lineDir.magnitude;
        if (lineLength < 0.001f) return false;

        lineDir.Normalize();
        float t = Mathf.Clamp01(Vector3.Dot(enemyPos - start, lineDir) / lineLength);
        Vector3 closest = start + lineDir * t;

        float dist = Vector3.Distance(enemyPos, closest);
        return dist <= bladeRadius;
    }

    void OnDrawGizmos()
    {
        if (!debugGizmos) return;
        if (bladeStart == null || bladeEnd == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(bladeStart.position, bladeRadius);
        Gizmos.DrawWireSphere(bladeEnd.position, bladeRadius);
        Gizmos.DrawLine(bladeStart.position, bladeEnd.position);
    }

    // ================== API NÂNG CẤP ==================
    public float GetRotateSpeed() => rotateSpeed;
    public float GetDamage() => damage;
    public int GetAbilityLevel() => abilityLevel;

    public void IncreaseRotateSpeed(float amount)
    {
        rotateSpeed += amount;
        rotateSpeed = Mathf.Clamp(rotateSpeed, 0f, 720f);
        abilityLevel += 1;
    }

    public void IncreaseDamage(float amount)
    {
        damage += amount;
        abilityLevel += 1;
    }
}
