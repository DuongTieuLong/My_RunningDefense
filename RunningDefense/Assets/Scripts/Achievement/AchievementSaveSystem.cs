using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;

[System.Serializable]
public class AchievementSaveData
{
    public string id;
    public int currentValue;
    public bool isClaimed;
}

public static class AchievementSaveSystem
{
    private static string filePath => Application.persistentDataPath + "/achievements.json";

    public static void Save(List<AchievementData> achievements)
    {
        var list = achievements.Select(a => new AchievementSaveData
        {
            id = a.id,
            currentValue = a.currentValue,
            isClaimed = a.isClaimed
        }).ToList();

        string json = JsonUtility.ToJson(new Wrapper { list = list });
        File.WriteAllText(filePath, json);
    }

    public static void Load(List<AchievementData> achievements)
    {
        if (!File.Exists(filePath)) return;
        string json = File.ReadAllText(filePath);
        var wrapper = JsonUtility.FromJson<Wrapper>(json);
        foreach (var saved in wrapper.list)
        {
            var a = achievements.Find(x => x.id == saved.id);
            if (a != null)
            {
                a.currentValue = saved.currentValue;
                a.isClaimed = saved.isClaimed;
            }
        }
    }

    [System.Serializable]
    private class Wrapper { public List<AchievementSaveData> list; }
}
