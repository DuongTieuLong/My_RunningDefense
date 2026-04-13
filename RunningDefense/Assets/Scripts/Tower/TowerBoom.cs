using System.Collections.Generic;
using UnityEngine;

public class TowerBoom : MonoBehaviour
{
    [SerializeField] private float radius;    // bán kính nổ
    [SerializeField] private float damage;      // sát thương gây ra
    [SerializeField] private float damageMultiplier = 2f; // hệ số sát thương
    [SerializeField] private TowerStats  towerStats;
    [SerializeField] private AudioSource AudioSource;

    public ParticleSystem explodeEff;
    public AudioClip ExplodeSound;

    private void Start()
    {
        towerStats = GetComponent<TowerStats>();
        explodeEff.gameObject.SetActive(false);
    }

    public void GetStats()
    {
        radius = towerStats.baseAttackRadius;
        damage = towerStats.baseDamage; 

    }

    public void Explode()
    {
        if(transform == null) return;
        if (explodeEff != null && explodeEff.isStopped) 
        {
            if (explodeEff.gameObject.activeSelf)
            {
                explodeEff.gameObject.SetActive(false);
                explodeEff.gameObject.SetActive(true);
            }
            else
            {
                explodeEff.gameObject.SetActive(true);
            }
            AudioSource.PlayOneShot(ExplodeSound, 5f);
        } 

         
        if (EnemyManager.Instance == null) return;
        
        List<Enemy> enemies = EnemyManager.Instance.ActiveEnemies;
        GetStats();
        foreach (var enemy in enemies)
        {
            if (enemy == null) continue;

            float dist = Vector3.Distance(transform.position, enemy.transform.position);
            if (dist <= radius)
            {
                enemy.TakeDamage(damage * damageMultiplier, gameObject);
            }
        }
    }
}
