using UnityEngine;
using System.Collections.Generic;

public class TowerAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    [SerializeField] public float attackRadius = 5f;
    [SerializeField] public float attackCooldown = 1f;
    public int attackDamage = 10;
    float attackDelay;

    [Header("Projectile Settings")]
    public GameObject projectilePrefab;
    public AudioClip attackSound;
    public AudioSource soundSource;
    public Transform firePoint;
    public Transform ammoLoad;
    public int poolSize = 20; // số lượng đạn trong pool

    private Queue<GameObject> bulletPool;

    [Header("Effects")]
    public RangeVisualizer turretRangeDrawer;
    public GameObject towerGun;

    // --- rotation settings (added) ---
    [Header("Rotation")]
    [Tooltip("Degrees/sec when attackCooldown == 1. Actual speed = baseRotationSpeed * (1 / attackCooldown)")]
    public float baseRotationSpeed = 180f;
    private Quaternion desiredRotation;
    private bool hasDesiredRotation = false;

    [Tooltip("Fallback yaw reference: Transform sẽ cung cấp góc Y ban đầu (lấy localEulerAngles.y) để tự động bù rotation model.")]
    public Transform yawReference; // thay cho gunYawOffset float
    private float initialGunYaw = 0f;

    void Start()
    {
        attackDelay = attackCooldown;
        if (turretRangeDrawer != null)
            turretRangeDrawer.SetRadius(attackRadius/2);

        // khởi tạo object pool
        bulletPool = new Queue<GameObject>();
        for (int i = 0; i < poolSize; i++)
        {
            GameObject bullet;
            if (ammoLoad != null)
                bullet = Instantiate(projectilePrefab, ammoLoad);
            else
                bullet = Instantiate(projectilePrefab);

            bullet.SetActive(false);
            bullet.transform.SetParent(ammoLoad != null ? ammoLoad : this.transform, false);
            bulletPool.Enqueue(bullet);
        }

        // lưu góc Y ban đầu từ yawReference (local) để dùng bù khi xoay
        if (yawReference != null)
            initialGunYaw = yawReference.localEulerAngles.y;
        else
            initialGunYaw = 0f;

        if (towerGun != null)
        {
            desiredRotation = towerGun.transform.rotation;
            hasDesiredRotation = true;
        }
    }

    void Update()
    {
        attackDelay -= Time.deltaTime;

        // Lấy danh sách enemy đang hoạt động từ EnemyManager thay vì Physics
        List<Enemy> enemies = EnemyManager.Instance.ActiveEnemies;

        // Tìm enemy gần nhất trong bán kính
        Enemy nearestEnemy = GetNearestEnemyInRange(enemies);

        // rotate turret towards current target (rotation speed scales with attack speed)
        RotateTurretTowards(nearestEnemy != null ? nearestEnemy.transform : null);

        if (nearestEnemy != null && attackDelay <= 0f)
        {
            if (soundSource != null)
                soundSource.PlayOneShot(attackSound);
            Shoot(nearestEnemy.transform);
            attackDelay = attackCooldown;
        }
    }

 //   public GameObject lastBullet;
    void Shoot(Transform target)
    {
        GameObject bullet = GetBulletFromPool();

        Vector3 spawnPos = (firePoint != null) ? firePoint.position : transform.position;
        Quaternion barrelRot = (firePoint != null) ? firePoint.rotation : transform.rotation;

        bullet.transform.position = spawnPos;

        if (target != null)
        {
            Vector3 aimDir = target.position - spawnPos;
            if (aimDir.sqrMagnitude > 0.0001f)
                bullet.transform.rotation = Quaternion.LookRotation(aimDir.normalized);
            else
                bullet.transform.rotation = barrelRot;
        }
        else
        {
            bullet.transform.rotation = barrelRot;
        }

        // nếu model nòng bị đảo, bù 180° quanh Y để hướng đạn đúng
        bullet.transform.rotation *= Quaternion.Euler(0f, 180f, 0f);

        bullet.SetActive(true);

        TowerBullet bulletScript = bullet.GetComponent<TowerBullet>();
        bulletScript.SetTarget(target, attackDamage, ReturnBulletToPool);
    }

    GameObject GetBulletFromPool()
    {
        foreach (var bullet in bulletPool)
        {
            if (!bullet.activeSelf)
            {
                bulletPool = new Queue<GameObject>(bulletPool); // rebuild queue giữ thứ tự
                return bullet;
            }
        }

        // nếu không có viên nào rảnh, tạo thêm
        GameObject newBullet = Instantiate(projectilePrefab, ammoLoad);
        newBullet.SetActive(false);
        bulletPool.Enqueue(newBullet);
        return newBullet;
    }

    void ReturnBulletToPool(GameObject bullet)
    {
        bullet.SetActive(false);
        bulletPool.Enqueue(bullet);
    }

    /// <summary>
    /// Tìm enemy gần nhất trong attackRadius
    /// </summary>
    Enemy GetNearestEnemyInRange(List<Enemy> enemies)
    {
        Enemy nearest = null;
        float minDist = attackRadius; // bắt đầu bằng bán kính
        foreach (var enemy in enemies)
        {
            if (enemy.isDead) continue; // bỏ qua enemy đã chết
            float dist = Vector3.Distance(transform.position, enemy.transform.position);
            if (dist <= attackRadius && dist < minDist)
            {
                minDist = dist;
                nearest = enemy;
            }
        }
        return nearest;
    }

    /// <summary>
    /// Xoay towerGun về hướng target. Sử dụng pointGun (nòng) để tính rotation đúng:
    /// desired towerGun.rotation = LookRotation(targetDir) * inverse(pointGun.localRotation)
    /// Điều này đảm bảo hướng world-forward của pointGun trùng với hướng bắn mong muốn.
    /// Nếu không có pointGun sẽ dùng fallback bằng gunYawOffset.
    /// </summary>
    private void RotateTurretTowards(Transform target)
    {
        if (towerGun == null) return;

        if (target != null)
        {
            Vector3 origin = towerGun.transform.position;
            Vector3 dir = target.position - origin;
            dir.y = 0f; // chỉ xoay yaw

            if (dir.sqrMagnitude > 0.0001f)
            {
                // Look rotation we want the barrel to face
                Quaternion look = Quaternion.LookRotation(dir.normalized);

                // luôn bù 180° vì barrel local-forward bị đảo + bù offset yaw model mặc định lấy từ initialGunYaw
                float totalYawCompensation = -initialGunYaw + 180f;

                desiredRotation = look * Quaternion.Euler(0f, totalYawCompensation, 0f);
                hasDesiredRotation = true;
            }
        }

        if (!hasDesiredRotation) return;

        float speedMultiplier = attackCooldown > 0f ? (1f / attackCooldown) : 1f;
        float rotationSpeed = baseRotationSpeed * speedMultiplier;
        rotationSpeed = Mathf.Clamp(rotationSpeed, 10f, 2000f);

        towerGun.transform.rotation = Quaternion.RotateTowards(
            towerGun.transform.rotation,
            desiredRotation,
            rotationSpeed * Time.deltaTime
        );
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
}
