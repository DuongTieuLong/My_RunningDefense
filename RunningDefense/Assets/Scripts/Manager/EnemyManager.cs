using UnityEngine;
using System.Collections.Generic;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance;

    [Header("Enemy Pools")]
    public List<EnemyPoolEntry> enemyEntries = new List<EnemyPoolEntry>();
    private Dictionary<string, ObjectPool<Enemy>> enemyPools = new Dictionary<string, ObjectPool<Enemy>>();

    [Header("Bullet Pools")]
    private ObjectPool<EnemyBullet> arrowBulletPool;
    private ObjectPool<EnemyBullet> magicBulletPool;

    private EnemyBullet arrowBulletPrefab;
    private EnemyBullet magicBulletPrefab;


    [Header("References")]
    public EnemyDifficultyScaler difficultyScaler;

    private readonly List<Enemy> activeEnemies = new List<Enemy>();
    public List<Enemy> ActiveEnemies => activeEnemies;
    public int ActiveEnemyCount => activeEnemies.Count;

    public Transform player;
    public Transform tower;

    public GameObject EnemyParent;

    public enum EnemyBulletType { Arrow, Magic }


    void Awake()
    {
        Instance = this;
        enemyEntries = GameDataManager.Instance.currentMapConfig.enemyPools;
    }

    void Start()
    {
        // tạo pool cho mỗi loại enemy
        foreach (var entry in enemyEntries)
        {
            if (entry.prefab != null)
                enemyPools[entry.id] = new ObjectPool<Enemy>(entry.prefab, entry.count, EnemyParent.transform);
        }

        // --- LẤY PREFAB TỪ MapEnemyConfig ---
        var config = GameDataManager.Instance.currentMapConfig;
        arrowBulletPrefab = config.arrowBulletPrefab;
        magicBulletPrefab = config.magicBulletPrefab;

        arrowBulletPool = new ObjectPool<EnemyBullet>(arrowBulletPrefab, config.arrowBulletCount, EnemyParent.transform);
        magicBulletPool = new ObjectPool<EnemyBullet>(magicBulletPrefab, config.magicBulletCount, EnemyParent.transform);
    }

    void Update()
    {
        if ((player == null || !player.gameObject.activeInHierarchy) &&
            (tower == null || !tower.gameObject.activeInHierarchy))
        {
            return;
        }

        for (int i = activeEnemies.Count - 1; i >= 0; i--)
        {
            if (i < activeEnemies.Count)
                activeEnemies[i].CustomUpdate();
        }
    }


    public EnemyBullet SpawnBullet(Vector3 pos, Vector3 dir, float dmg, EnemyBulletType type)
    {
        ObjectPool<EnemyBullet> pool = type == EnemyBulletType.Arrow ? arrowBulletPool : magicBulletPool;
        var b = pool.Get();
        b.transform.position = pos;
        b.Init(dir, dmg, () => pool.ReturnToPool(b));
        return b;
    }

    public void RegisterEnemy(Enemy e) => activeEnemies.Add(e);
    public void UnregisterEnemy(Enemy e) => activeEnemies.Remove(e);

    public Enemy SpawnEnemy(string id, Vector3 pos)
    {
        if (!enemyPools.ContainsKey(id))
        {
            return null;
        }

        var pool = enemyPools[id];
        var e = pool.Get();

        float prefabY = e.gameObject.transform.position.y;
        e.transform.position = new Vector3(pos.x, pos.y + prefabY, pos.z);

        // Lấy độ khó hiện tại
        var scale = difficultyScaler != null ? difficultyScaler.GetCurrentScale() : new DifficultyStats();

        e.maxHealth *= scale.healthMultiplier;
        e.attackDamage *= scale.damageMultiplier;
        e.moveSpeed *= scale.speedMultiplier;

        e.Init(() => pool.ReturnToPool(e));

        return e;
    }

    public Transform GetPlayer() => player;
    public Transform GetTower() => tower;
}
