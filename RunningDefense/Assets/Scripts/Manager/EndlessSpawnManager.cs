using UnityEngine;

public class EndlessSpawnManager : MonoBehaviour
{
    [Header("References")]
    public EnemyManager enemyManager;
    public Transform[] spawnPoints;

    [Header("Enemy IDs (match EnemyManager)")]
    public string normalEnemy1_ID = "Normal1";
    public string normalEnemy2_ID = "Normal2";
    public string specialEnemy1_ID = "Special1";
    public string specialEnemy2_ID = "Special2";
    public string bossEnemy_ID = "Boss1";

    [Header("Spawn Settings")]
    public float baseSpawnInterval = 2f;
    public float spawnRateDecrease = 0.1f;      // giảm thời gian giữa các lượt spawn
    public int maxEnemiesOnField = 100;

    [Header("Time Milestones (minutes)")]
    public float specialEnemyStart = 5f;        // sau 5 phút có quái đặc biệt
    public float bossSpawnInterval = 10f;       // mỗi 10 phút ra boss

    private float spawnTimer;
    private float spawnCooldown;
    private float elapsedTime;
    private EnemyDifficultyScaler difficultyScaler;

    private int lastBossMinute = -1;

    void Start()
    {
        if (enemyManager == null)
            enemyManager = EnemyManager.Instance;

        difficultyScaler = GetComponent<EnemyDifficultyScaler>();
        spawnCooldown = baseSpawnInterval;
        spawnTimer = 0;
        elapsedTime = 0;

    }

    void Update()
    {
        elapsedTime += Time.deltaTime;
        spawnTimer += Time.deltaTime;

        // cập nhật độ khó
        if (difficultyScaler != null)
            difficultyScaler.UpdateDifficulty(elapsedTime);

        // điều chỉnh tốc độ spawn theo thời gian
        spawnCooldown = Mathf.Max(0.5f, baseSpawnInterval - (elapsedTime / 60f) * spawnRateDecrease);

        if (spawnTimer >= spawnCooldown)
        {
            spawnTimer = 0f;
            TrySpawnEnemies();
        }
    }

    void TrySpawnEnemies()
    {
        if (enemyManager.ActiveEnemyCount >= maxEnemiesOnField)
            return;

        float minutes = elapsedTime / 60f;

        // boss wave
        if (minutes >= bossSpawnInterval && Mathf.FloorToInt(minutes) % (int)bossSpawnInterval == 0 && lastBossMinute != Mathf.FloorToInt(minutes))
        {
            lastBossMinute = Mathf.FloorToInt(minutes);
            SpawnBossWave();
        }
        // special wave
        else if (minutes >= specialEnemyStart)
        {
            SpawnSpecialWave();
        }
        // normal wave
        else
        {
            SpawnNormalWave();
        }
    }

    void SpawnNormalWave()
    {
        int spawnCount = Mathf.Clamp(3 + Mathf.FloorToInt(elapsedTime / 60f), 3, 15);
        for (int i = 0; i < spawnCount; i++)
        {
            string id = Random.value > 0.5f ? normalEnemy1_ID : normalEnemy2_ID;
            SpawnEnemy(id);
        }
    }

    void SpawnSpecialWave()
    {
        int spawnCount = Mathf.Clamp(2 + Mathf.FloorToInt(elapsedTime / 120f), 2, 8);
        for (int i = 0; i < spawnCount; i++)
        {
            string id = Random.value > 0.5f ? specialEnemy1_ID : specialEnemy2_ID;
            SpawnEnemy(id);
        }
    }

    void SpawnBossWave()
    {
        SpawnEnemy(bossEnemy_ID);
    }

    void SpawnEnemy(string id)
    {
        if (spawnPoints.Length == 0) return;

        Transform point = spawnPoints[Random.Range(0, spawnPoints.Length)];
        Vector3 pos = point.position;

        enemyManager.SpawnEnemy(id, pos);
    }
}
