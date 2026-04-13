using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MapEnemyConfig", menuName = "Game Data/Map Enemy Config", order = 1)]
public class MapEnemyConfig : ScriptableObject
{
    [Header("Map Data Reference")]
    public MapAssetTable mapAssetTable;

    [Header("Enemy Pool Entries")]
    public List<EnemyPoolEntry> enemyPools = new List<EnemyPoolEntry>();

    [Header("Bullet Prefabs")]
    public EnemyBullet arrowBulletPrefab;
    public EnemyBullet magicBulletPrefab;

    [Header("Bullet Pool Settings")]
    public int arrowBulletCount = 200;
    public int magicBulletCount = 200;
}
