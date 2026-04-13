using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MatchStats
{
    // Tổng số quái bị giết (tính tất cả loại)
    public int TotalEnemiesKilled { get; private set; }

    // Tổng tiền kiếm được
    public int TotalGoldEarned { get; private set; }

    // Thống kê chi tiết theo từng loại quái
    private Dictionary<EnemyRank, int> enemiesKilledByType = new Dictionary<EnemyRank, int>();


    public MatchStats()
    {
        ResetStats();
    }

    /// <summary>
    /// Gọi khi một quái bị giết.
    /// </summary>
    public void AddKill(EnemyRank type, int goldReward)
    {
        TotalEnemiesKilled++;
        TotalGoldEarned += goldReward;

        if (!enemiesKilledByType.ContainsKey(type))
            enemiesKilledByType[type] = 0;

        enemiesKilledByType[type]++;
    }

    /// <summary>
    /// Lấy số quái giết được theo loại.
    /// </summary>
    public int GetKillsByType(EnemyRank type)
    {
        return enemiesKilledByType.TryGetValue(type, out int count) ? count : 0;
    }

    /// <summary>
    /// Reset lại toàn bộ dữ liệu — dùng khi bắt đầu trận mới.
    /// </summary>
    public void ResetStats()
    {
        TotalEnemiesKilled = 0;
        TotalGoldEarned = 0;
        enemiesKilledByType = new Dictionary<EnemyRank, int>();
    }

}
