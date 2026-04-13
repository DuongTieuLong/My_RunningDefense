using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SupplyPoolingManager : MonoBehaviour
{
    [Header("Setup")]
    public Supply supplyPrefab;        // Prefab Supply
    public int poolSize = 5;           // Số lượng trong pool
    public float radius = 10f;         // Bán kính spawn
    public float respawnTime = 5f;     // Thời gian hồi sinh

    private Queue<Supply> pool = new Queue<Supply>();
    public float showDuration = 2f; // tốc độ xoay
    [Header("Supply Weights: Player Heal, Tower Heal, Tower Boom")]
    public float [] weights = new float[3] { 0.5f, 0.3f, 0.2f }; // xác suất rơi

    [Header("Icon Mapping")]
    public List<SupplyIconData> iconDataList;
    private Dictionary<SupplyType, Sprite> iconDict = new Dictionary<SupplyType, Sprite>();

    [Header("References")]
    [HideInInspector] public PlayerStats playerStats;
    [HideInInspector] public Transform towerTranform;
    [HideInInspector] public TowerHealth towerHeath;
    [HideInInspector] public TowerBoom towerBoom;
    [HideInInspector] public PlayerReferences playerReferences;

    void Awake()
    {
        foreach (var data in iconDataList)
        {
            if (!iconDict.ContainsKey(data.type))
                iconDict.Add(data.type, data.icon);
        }
        playerStats = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();
        playerReferences = playerStats.GetComponent<PlayerReferences>();
        towerTranform = GameObject.FindGameObjectWithTag("Tower").transform;
        towerHeath = towerTranform.GetComponent<TowerHealth>();
        towerBoom = towerTranform.GetComponent<TowerBoom>();
 
    }

    public Sprite GetSupplyIcon(SupplyType type)
    {
        return iconDict[type];
    }

    void Start()
    {
        // Khởi tạo pool
        for (int i = 0; i < poolSize; i++)
        {
            Supply supply = Instantiate(supplyPrefab, transform);
            supply.gameObject.SetActive(false);
            supply.Init(this, weights);
            pool.Enqueue(supply);
        }

        // Bắt đầu vòng lặp spawn
        StartCoroutine(SpawnLoop());
    }

    private IEnumerator SpawnLoop()
    {
        while (true) // vòng lặp vĩnh viễn
        {
            yield return new WaitForSeconds(respawnTime);

            if (pool.Count > 0) // chỉ spawn khi pool còn item
            {
                SpawnSupply();
            }
        }
    }

    public void SpawnSupply()
    {
        Supply supply = pool.Dequeue();

        // Random trong bán kính
        Vector3 randomPos = transform.position + (Random.insideUnitSphere * radius);
        randomPos.y = 1.5f; // cao so với mặt đất, anh chỉnh lại theo game

        supply.transform.position = randomPos;
        supply.gameObject.SetActive(true);
    }

    // Gọi khi nhặt → trả về pool
    public void RePoolSupply(Supply supply)
    {
        supply.gameObject.SetActive(false);
        pool.Enqueue(supply);
    }
}

[System.Serializable]
public class SupplyIconData
{
    public SupplyType type;
    public Sprite icon;
}