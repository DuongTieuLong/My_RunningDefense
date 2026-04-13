using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrustMeleeWeapon : MonoBehaviour
{
    [Header("References")]
    public Transform player;       // Player để bám theo
    public PlayerStats playerStats;
    public Transform weaponModel;  // Mô hình vũ khí (kiếm, giáo...)
    public Transform attackPoint;  // Điểm gốc vùng attack

    [Header("Weapon Settings")]
    public string abilityName;
    public Sprite icon;
    [SerializeField] private int abilityLevel = 1;
    public float range = 5f;            // Tầm phát hiện enemy
    public float thrustDistance = 3f;   // Độ dài thrust
    public float thrustSpeed = 8f;      // Tốc độ thrust
    public float thurstholdTime = 0.2f; // Thời gian giữ nguyên vị trí sau khi đâm
    public Transform bladeStart;       // Điểm bắt đầu lưỡi kiếm
    public Transform bladeEnd;         // Điểm kết thúc lưỡi kiếm
    public float bladeRadius = 0.5f;   // Bán kính lưỡi kiếm
    [SerializeField] private float cooldownTime = 1f;     // Thời gian hồi sau mỗi đâm
    [SerializeField] private float damage = 10;           // Sát thương
    [SerializeField] private float height = 1f;           // Chiều cao bám theo player
    public AudioClip sfx;
    public GameObject trailEffect;

    [Header("Debug")]
    public bool debugGizmos = true;

    private float nextCooldownTime = 0f;
    private Vector3 localStart;
    private HashSet<Enemy> hitEnemies = new HashSet<Enemy>();
    private List<Enemy> inRange = new List<Enemy>();

    public GameObject playerOwner;

    void Start()
    {
        if (weaponModel != null)
            localStart = weaponModel.localPosition;

        playerOwner = GameObject.FindGameObjectWithTag("Player");
        playerStats = player.GetComponent<PlayerStats>();
    }

    void Update()
    {
        if (player == null || weaponModel == null) return;

        // Luôn bám theo vị trí player
        transform.position = player.position + new Vector3(0, height, 0);

        // Chỉ có thể attack khi hết cooldown
        if (Time.time >= nextCooldownTime)
        {
            Enemy target = FindNearestEnemy();
            if (target != null)
            {
                StartCoroutine(ThrustAttack(target));
                nextCooldownTime = Time.time + cooldownTime;
            }
        }
    }

    // --- Dùng EnemyManager ---
    Enemy FindNearestEnemy()
    {
        Enemy nearestEnemy = GetNearestEnemyInRange(GetAllEnemyInRange(), range);
        return nearestEnemy;
    }

    List<Enemy> GetAllEnemyInRange()
    {
        inRange.Clear();
        List<Enemy> enemies = EnemyManager.Instance.ActiveEnemies;
        foreach (var enemy in enemies)
        {
            if (enemy.isDead) continue; // bỏ qua enemy đã chết
            float dist = Vector3.Distance(transform.position, enemy.transform.position);
            if (dist < range)
            {
                inRange.Add(enemy);
            }
        }
        return inRange;
    }

    Enemy GetNearestEnemyInRange(List<Enemy> enemies, float radius)
    {
        Enemy nearest = null;
        float minDist = radius; // bắt đầu bằng bán kính
        foreach (var enemy in enemies)
        {
            float dist = Vector3.Distance(transform.position, enemy.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = enemy;
            }
        }
        return nearest;
    }

    IEnumerator ThrustAttack(Enemy target)
    {
        hitEnemies.Clear();

        // Xoay vũ khí về hướng target
        Vector3 dir = (target.transform.position - player.position).normalized;
        Quaternion thrustRot = Quaternion.LookRotation(dir, Vector3.up);
        transform.rotation = thrustRot;
        if (trailEffect != null)
            trailEffect.SetActive(true);

        // Bật model
        weaponModel.gameObject.SetActive(true);
        // Thrust ra → Lerp dần
        Vector3 start = localStart;
        Vector3 end = start + Vector3.forward * thrustDistance;
        float t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime * thrustSpeed;
            weaponModel.localPosition = Vector3.Lerp(start, end, t);
            EnemiesTrigger();
            yield return null;
        }
        EnemiesTrigger();
        yield return new WaitForSeconds(thurstholdTime);

        // Tắt model
        weaponModel.gameObject.SetActive(false);
        if (trailEffect != null)
            trailEffect.transform.rotation = thrustRot;
        trailEffect.SetActive(false);

        // Reset vị trí vũ khí
        weaponModel.localPosition = localStart;
    }

    List<Enemy> toDamage = new List<Enemy>();
    void EnemiesTrigger()
    {
        if (bladeStart == null || bladeEnd == null) return;
        if (EnemyManager.Instance == null) return;
        inRange = GetAllEnemyInRange();
        foreach (var enemy in inRange)
        {
            if (enemy == null) continue;
            if (IsEnemyInBladeRange(enemy) && hitEnemies.Add(enemy))
                toDamage.Add(enemy);
        }
        foreach (var enemy in toDamage)
        {
            float finalDamage = damage + playerStats.atkPoint;
            SoundManager.Instance.PlaySFX(sfx);
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

        if (!debugGizmos || player == null) return;
        // vẽ viền
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(player.position, range);
    }

    // ========================
    // Buff từ ngoài
    // ========================
    public float GetCooldown() => cooldownTime;
    public float GetDamage() => damage;
    public int GetAbilityLevel() => abilityLevel;


    public void IncreaseDamage(float amount)
    {
        damage += amount;
        abilityLevel += 1;
    }

    public void ReduceCooldown(float amount)
    {
        cooldownTime = Mathf.Max(0.3f, cooldownTime - amount);
        abilityLevel += 1;
    }

}
