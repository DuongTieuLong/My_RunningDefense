using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeWeapon : MonoBehaviour
{
    [Header("References")]
    public Transform player;        // Player để bám theo
    public PlayerStats playerStats;
    public Transform firePoint;     // Nơi bắn đạn ra
    public GameObject bulletPrefab; // Prefab viên đạn
    public AudioClip sfx;

    [Header("Weapon Settings")]
    public string abilityName;
    public Sprite icon;
    [SerializeField] private int abilityLevel = 1;
    [SerializeField] private float range = 5f;         // Tầm phát hiện enemy
    public float cooldownTime = 2f;   // Thời gian hồi (mặc định 2s)
    [SerializeField] private float damage = 10;           // Sát thương mỗi viên
    [SerializeField] private int bulletPerShot = 1;     // Số lượng đạn mỗi lần bắn
    [SerializeField] private float height = 1f;        // Chiều cao bám theo player
    public float bulletSpeed = 15f;   // Tốc độ bay của đạn
    public int poolSize = 20;        // Kích thước pool đạn
    public Transform ammoLoad;       // Nơi chứa đạn trong hierarchy
    public LayerMask enemyLayer;

    [Header("Debug")]
    public bool debugGizmos = true;

    private float nextCooldownTime = 0f;


    private Queue<GameObject> bulletPool;

    public RangeVisualizer rangeVisualizer;

    private void Start()
    {
        // khởi tạo object pool
        bulletPool = new Queue<GameObject>();
        for (int i = 0; i < poolSize; i++)
        {
            GameObject bullet = Instantiate(bulletPrefab);
            bullet.SetActive(false);
            bullet.transform.parent = ammoLoad; // để gọn trong hierarchy
            bulletPool.Enqueue(bullet);
        }

        rangeVisualizer.SetRadius(range);
        playerStats = player.GetComponent<PlayerStats>();
    }
    void Update()
    {
        if (player == null || firePoint == null) return;

        // Luôn bám theo player
        transform.position = player.position + new Vector3(0, height, 0);

        // Nếu hết cooldown thì bắn
        if (Time.time >= nextCooldownTime)
        {
            List<Enemy> enemies = EnemyManager.Instance.ActiveEnemies;
            Enemy target = GetNearestEnemyInRange(enemies);
            if (target != null)
            {
                StartCoroutine(Shoot(target.transform));
                nextCooldownTime = Time.time + cooldownTime;
            }
        }
    }

    Enemy GetNearestEnemyInRange(List<Enemy> enemies)
    {
        Enemy nearest = null;
        float minDist = range; // bắt đầu bằng bán kính
        foreach (var enemy in enemies)
        {
            if(enemy.isDead ) continue; // bỏ qua enemy đã chết
            float dist = Vector3.Distance(transform.position, enemy.transform.position);
            if ( dist < minDist)
            {
                minDist = dist;
                nearest = enemy;
            }
        }
        return nearest;
    }

    IEnumerator Shoot(Transform target)
    {
        // Xoay về phía enemy
        Vector3 dir = (target.transform.position - firePoint.position).normalized;
        Quaternion rot = Quaternion.LookRotation(dir, Vector3.up);
        transform.rotation = rot;

        for (int i = 0; i < bulletPerShot; i++)
        {
            GameObject bullet = GetBulletFromPool();
            bullet.transform.position = firePoint.position;
            bullet.transform.rotation = rot;
            bullet.SetActive(true);

            // Gán damage cho bullet
            BulletRangeWeapon b = bullet.GetComponent<BulletRangeWeapon>();
            if (b != null)
            {
                float finalDamage = damage + playerStats.atkPoint; // Sát thương = sát thương vũ khí + sát thương player
                b.SetStart(finalDamage, target, bulletSpeed, ReturnBulletToPool);
                SoundManager.Instance.PlaySFX(sfx);
            }
            yield return new WaitForSeconds(0.1f); // delay giữa các viên trong cùng 1 phát bắn
        }
    }

    void ReturnBulletToPool(GameObject bullet)
    {
        bullet.SetActive(false);
        bulletPool.Enqueue(bullet);
    }

    GameObject GetBulletFromPool()
    {
        if (bulletPool.Count > 0)
        {
            return bulletPool.Dequeue();
        }
        else
        {
            // Nếu hết bullet trong pool thì tạo thêm 
            GameObject bullet = Instantiate(bulletPrefab);
            return bullet;
        }
    }
    // Vẽ tầm phát hiện
    void OnDrawGizmos()
    {
        if (!debugGizmos || player == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(player.position, range);
    }

    // ========================
    // Các hàm buff từ ngoài
    // ========================
    public float GetDamage() => damage;
    public float GetRange() => range;
    public int GetBulletPerShot() => bulletPerShot;
    public int GetAbilityLevel() => abilityLevel;   

    public float Range { get { return range; } }
    public int BulletPerShot { get { return bulletPerShot; } }
    public void IncreaseDamage(float amount)
    {
        damage += amount;
        abilityLevel += 1;
    }

    public void IncreaseRange(float range)
    {
        this.range += range;
        abilityLevel += 1;
        rangeVisualizer.SetRadius(this.range);
    }


    public void IncreaseBulletPerShot(int amount)
    {
        bulletPerShot += amount;
        abilityLevel += 1;
    }
}
