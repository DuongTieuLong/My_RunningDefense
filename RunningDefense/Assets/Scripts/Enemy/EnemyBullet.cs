using DG.Tweening;
using System;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public GameObject effectObj;
    private ParticleSystem hit;
    private ParticleSystem flash;

    public float speed = 10f;
    public float lifeTime = 3f;
    public float maxDistance = 20f;
    public float hitRadius = 0.5f;

    private Vector3 direction;
    private float damage;
    private Action onReturnToPool;
    private float spawnTime;
    private Vector3 spawnPos;

    private Transform player;
    private Transform tower;

    public void Init(Vector3 dir, float damage, Action onReturnToPool)
    {
        oneHit = true;
        this.direction = dir.normalized;
        this.damage = damage;
        this.onReturnToPool = onReturnToPool;
        spawnTime = Time.time;
        spawnPos = transform.position;

        player = EnemyManager.Instance.GetPlayer();
        tower = EnemyManager.Instance.GetTower();

        if (effectObj != null && effectObj.transform.childCount >= 4)
        {
            hit = effectObj.transform.GetChild(2).GetComponent<ParticleSystem>();
            flash = effectObj.transform.GetChild(3).GetComponent<ParticleSystem>();
        }
        else
        {
            hit = null;
            flash = null;
        }
    }


    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;

        // Xoay mũi tên về phía target
        if (direction != Vector3.zero)
        {
            Quaternion lookRot = Quaternion.LookRotation(direction);
            transform.rotation = lookRot * (Quaternion.Euler(-90f, 0f, 0f));
        }

        if (TryHit())
        {
            direction = Vector3.zero; // Dừng di chuyển
            if (flash != null)
                flash.Play();
            if (hit != null)
                hit.Play();
            DOVirtual.DelayedCall(0.2f, () =>
            {
                onReturnToPool?.Invoke();
            }).SetLink(gameObject);
            return;
        }

        if (Time.time - spawnTime > lifeTime ||
            Vector3.Distance(spawnPos, transform.position) > maxDistance)
        {
            onReturnToPool?.Invoke();
        }
    }

    private bool oneHit = true;
    private bool TryHit()
    {
        if (direction == Vector3.zero) return false;

        if (oneHit && player != null && Vector3.Distance(transform.position - new Vector3(0, 0.5f, 0), player.position) <= hitRadius)
        {
            oneHit = false;
            if (player.TryGetComponent<PlayerStats>(out var ps)) ps.TakeDamge(damage);
            return true;
        }
        if (oneHit && tower != null && Vector3.Distance(transform.position, tower.position) <= hitRadius)
        {
            oneHit = false;
            if (tower.TryGetComponent<TowerHealth>(out var th)) th.TakeDamage(damage);
            return true;
        }
        return false;
    }
}
