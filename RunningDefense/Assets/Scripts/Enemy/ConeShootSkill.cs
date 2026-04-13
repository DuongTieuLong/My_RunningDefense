using UnityEngine;

public class ConeShootSkill : MonoBehaviour
{
    public int projectileCount = 5;
    public float coneAngle = 45f;
    public float cooldown = 8f;
    public AudioClip shootSFX;
    public ParticleSystem shootVFX;

    private float lastShootTime;
    private Animator animator;
    private Enemy enemy;
    private Transform player;

    void Start()
    {
        enemy = GetComponent<Enemy>();
        player = EnemyManager.Instance.GetPlayer();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (player == null) return;

        if (Time.time - lastShootTime > cooldown)
        {
            lastShootTime = Time.time;
            ShootConeProjectiles();
        }
    }

    void ShootConeProjectiles()
{
    Vector3 baseDir = (player.position - transform.position).normalized;
    baseDir.y = 0;

    for (int i = 0; i < projectileCount; i++)
    {
        float angle = Mathf.Lerp(-coneAngle / 2f, coneAngle / 2f, (float)i / (projectileCount - 1));
        Vector3 dir = Quaternion.Euler(0, angle, 0) * baseDir;

        // Bắn loại đạn mũi tên (Arrow)
        EnemyManager.Instance.SpawnBullet(
            transform.position + Vector3.up * 1.2f,
            dir,
            enemy.attackDamage,
            EnemyManager.EnemyBulletType.Arrow
        );
    }

    if (shootSFX)
        SoundManager.Instance.PlaySFX(shootSFX);

    if (shootVFX)
        shootVFX.Play();

    if (animator)
        animator.SetTrigger("Attack");
}

}
