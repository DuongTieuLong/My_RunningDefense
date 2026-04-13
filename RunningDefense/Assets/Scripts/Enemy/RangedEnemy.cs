using UnityEngine;

public class RangedEnemy : Enemy
{
    public float attackRange = 6f;
    public AudioClip atkSFX;
    public bool isArrow = true;

    public override void CustomUpdate()
    {
        if (isDead) return;

        Transform target = GetPriorityTarget();

        // Nếu không có target hoặc target không còn active, ngừng hoạt động
        if (target == null || !target.gameObject.activeInHierarchy)
        {
            PlayMoveAnim(false);
            return;
        }

        // Hướng di chuyển về target (có tránh va chạm với enemy khác)
        Vector3 dir = target.position - transform.position;
        dir += SeparateFromOthers();
        LookAtTarget(dir);

        float distToTarget = Vector3.Distance(transform.position, target.position);

        // Nếu chưa tới tầm bắn -> di chuyển
        if (distToTarget > attackRange)
        {
            MoveTowards(dir);
            PlayMoveAnim(true);
        }
        else
        {
            // Đã trong tầm -> đứng bắn
            PlayMoveAnim(false);

            if (Time.time - lastAttackTime >= attackRate)
            {
                lastAttackTime = Time.time; // Ghi lại thời điểm bắn

                if (target == null || !target.gameObject.activeInHierarchy)
                    return;

                Vector3 shootDir = (target.position - transform.position + new Vector3(0, 0.5f, 0)).normalized;

                // Bắn đạn
                var bulletType = isArrow ? EnemyManager.EnemyBulletType.Arrow : EnemyManager.EnemyBulletType.Magic;
                EnemyManager.Instance.SpawnBullet(transform.position, shootDir, attackDamage, bulletType);

                // Animation + âm thanh
                PlayAttackAnim();
                if (atkSFX != null)
                    SoundManager.Instance.PlaySFX(atkSFX);
            }
        }
    }
}
