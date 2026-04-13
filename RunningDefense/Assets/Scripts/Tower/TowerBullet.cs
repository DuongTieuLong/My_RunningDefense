using DG.Tweening;
using System;
using UnityEngine;

public class TowerBullet : MonoBehaviour
{
    public float speed = 10f;
    private Transform target;
    private int damage;
    private Action<GameObject> returnToPool;
    public ParticleSystem hitEffect;
    public GameObject towerOwner;

    public void SetTarget(Transform target, int damage, Action<GameObject> returnBullet)
    {
        this.target = target;
        this.damage = damage;
        this.returnToPool = returnBullet;
        this.towerOwner = GameObject.FindGameObjectWithTag("Tower");
    }


    void Update()
    {
        if (target == null)
        {
            // Trả về pool nếu target chết/mất
            returnToPool?.Invoke(gameObject);
            return;
        }

        // bay theo target
        Vector3 dir = target.position - transform.position;
        float distanceThisFrame = speed * Time.deltaTime;

        if (dir.magnitude <= distanceThisFrame)
        {
            HitTarget();
            return;
        }

        transform.Translate(dir.normalized * distanceThisFrame, Space.World);
    }

    private bool oneHit = true;
    private bool oneDealDamage = true;
    void HitTarget()
    {
        if (hitEffect != null && oneHit)
        {
            oneHit = false;
            Instantiate(hitEffect, transform.position, Quaternion.identity);
        }
        // Gây damage cho enemy ở đây
        Enemy enemy = target.GetComponent<Enemy>();
        if (enemy != null && oneDealDamage)
        {
            oneDealDamage = false;
            enemy.TakeDamage(damage, towerOwner); // towerOwner = trụ đã bắn
        }

        // Trả lại bullet vào pool
        DOVirtual.DelayedCall(0.5f, () =>
        {
            oneHit = true;
            oneDealDamage = true;
            returnToPool?.Invoke(gameObject);
        }).SetLink(gameObject);
    }
}
