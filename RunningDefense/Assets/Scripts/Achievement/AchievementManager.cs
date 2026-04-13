using System;
using System.Collections.Generic;
using UnityEngine;

public class AchievementManager : MonoBehaviour
{
    public static AchievementManager Instance;

    private Dictionary<string, AchievementData> progress = new Dictionary<string, AchievementData>();

    public List<AchievementData> achievements;


    private void Awake()
    {
        if (Instance == null) Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        AchievementSaveSystem.Load(achievements);
        foreach (var achievement in achievements)
        {
            if (progress.ContainsKey(achievement.id)) continue;
            progress[achievement.id] = achievement;
        }
    }

    public void AddProgress(AchievementType type, int amount = 0)
    {
        foreach (var achievement in achievements)
        {
            if (achievement.type != type) continue;
            if(progress[achievement.id].currentValue >= achievement.targetValue)
                continue;
            progress[achievement.id].currentValue += amount;
            if(progress[achievement.id].currentValue > achievement.targetValue)
            {
                progress[achievement.id].currentValue = achievement.targetValue;
            }
            AchievementSaveSystem.Save(achievements);
        }
    }

    public void Claimed(AchievementData achievement)
    {
        achievement.isClaimed = true;
        AchievementSaveSystem.Save(achievements);
    }

#if UNITY_EDITOR
    [ContextMenu("Reset All Achievements")]
    public void ResetAllAchievements()
    {
        foreach (var achievement in achievements)
        {
            achievement.currentValue = 0;
            achievement.isClaimed = false;
        }
        AchievementSaveSystem.Save(achievements);
    }
#endif

}
