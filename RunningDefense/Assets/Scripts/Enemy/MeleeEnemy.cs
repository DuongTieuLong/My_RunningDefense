using UnityEngine;

public class MeleeEnemy : Enemy
{
    public float attackDistance = 1.5f;
    public AudioClip atkSFX;

    public override void CustomUpdate()
    {
        if (isDead) return;
        Transform target = GetPriorityTarget();
        if (target == null) return;

        Vector3 dir = target.position - transform.position;
        dir += SeparateFromOthers();

        LookAtTarget(dir);

        float distToTarget = Vector3.Distance(transform.position, target.position);

        if (distToTarget > attackDistance)
        {
            MoveTowards(dir);
            PlayMoveAnim(true);
        }
        else
        {
            PlayMoveAnim(false);

            if (Time.time - lastAttackTime > attackRate)
            {
                PlayAttackAnim();
                SoundManager.Instance.PlaySFX(atkSFX);
                if (target.TryGetComponent<PlayerStats>(out var playerStats))
                    playerStats.TakeDamge(attackDamage);
                else if (target.TryGetComponent<TowerHealth>(out var towerHealth))
                    towerHealth.TakeDamage(attackDamage);

                lastAttackTime = Time.time;
            }
        }
    }
}
