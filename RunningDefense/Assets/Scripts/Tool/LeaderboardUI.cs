using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardUI : MonoBehaviour
{
    [Header("References")]
    public Transform contentParent;          // Content trong ScrollView
    public GameObject leaderboardItemPrefab; // Prefab UI hiển thị mỗi dòng (Rank, Name, Time)
    public GameObject playerItem;

    [Header("Sample Player Data")]
    public List<string> playerNames;

    public List<float> playerTimes = new List<float>();

    [Header("Random Time Range (Seconds)")]
    public float minTime = 60f;   // 1 phút
    public float maxTime = 300f;  // 10 phút
    public float realMaxTime = 1000f;

    List<PlayerRecord> records = new List<PlayerRecord>();

    public string playerName;

    public float bestTime;
    private class PlayerRecord
    {
        public string Name;
        public float Time;
    }

    public void Start()
    {
        playerNames = new List<string>
        {
        "Rity", "Alex", "Nova", "Kane", "Mira", "Taro", "Luna", "Orin", "Eve", "Zane",
        "Nero", "Ava", "Soren", "Iris", "Blake", "Theo", "Kai", "Lara", "Noah", "Milo",
        "Selene", "Drake", "Raven", "Echo", "Vex", "Cora", "Finn", "Nia", "Juno", "Arin",
        "Dara", "Elio", "Rhea", "Vale", "Rin", "Sylas", "Tess", "Orion", "Nyx", "Cael",
        "Pax", "Lyra", "Ezra", "Cass", "Jade", "Remy", "Vera", "Clyde", "Niko", "Xen"
        };
 

        playerName = PlayerPrefs.GetString("PlayerName", "");

        if (!string.IsNullOrEmpty(playerName))
        {
            bestTime = PlayerPrefs.GetFloat($"BestTime_{playerName}", 0f);
            playerNames.Add(playerName);
            playerTimes.Add(bestTime);
        }

        FakeRankUp();
        GenerateRandomTimes();

        for (int i = 0; i < Mathf.Min(playerNames.Count, playerTimes.Count); i++)
        {
            records.Add(new PlayerRecord
            {
                Name = playerNames[i],
                Time = playerTimes[i]
            });
        }
    }

    public void FakeRankUp()
    {
        if(PlayerPrefs.GetInt("OneCheck", 0) == 0)
        {
            if (bestTime > maxTime)
            {
                maxTime = bestTime + Random.Range(30, 90);
            }
            if (bestTime > realMaxTime)
            {
                bestTime = realMaxTime;
            }
            PlayerPrefs.SetInt("OneCheck", 1);
        }

    }

    public void OnApplicationQuit()
    {
        PlayerPrefs.DeleteKey("OneCheck");
    }

    public void GenerateRandomTimes()
    {
        playerTimes.Clear();

        for (int i = 0; i < playerNames.Count; i++)
        {
            if(playerNames[i] == playerName)
            {
                playerTimes.Add(bestTime);
                continue;
            }
            float randomTime = Random.Range(minTime, maxTime);
            playerTimes.Add(randomTime);
        }

    }

    public void GetNewValue()
    {
        if (string.IsNullOrEmpty(playerName))
        {
            playerName = PlayerPrefs.GetString("PlayerName", "");
        }

        var playerTime = PlayerPrefs.GetFloat($"BestTime_{playerName}", 0f);
        var existing = records.Find(r => r.Name == playerName);

        if (existing != null)
        {
            // Nếu đã có, cập nhật nếu tốt hơn
            if (existing.Time < playerTime)
                existing.Time = playerTime;
        }
        else
        {
            records.Add(new PlayerRecord { Name = playerName, Time = playerTime });
        }
    }

    public void UpdatePlayer(int index)
    {
        if (index <= 10)
        {
            playerItem.SetActive(false);
            return;
        }

        playerItem.SetActive(true);

        var rankText = playerItem.transform.Find("RankText")?.GetComponent<TextMeshProUGUI>();
        var nameText = playerItem.transform.Find("NameText")?.GetComponent<TextMeshProUGUI>();
        var timeText = playerItem.transform.Find("TimeText")?.GetComponent<TextMeshProUGUI>();

        if (rankText != null) rankText.text = $"{index}";
        if (nameText != null) nameText.text = playerName;
        if (timeText != null) timeText.text = FormatTime(bestTime);
    }

    public void GenerateLeaderboard()
    {
        // Xóa các item cũ trong Content trước khi tạo mới
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        GetNewValue();
       
        // Sắp xếp giảm dần theo thời gian 
        var sortedRecords = records.OrderByDescending(r => r.Time).ToList();

        int count;
        if (sortedRecords.Count > 10)
        {
            count = 10;
        }
        else
        {
            count = sortedRecords.Count;
        }


        // Spawn từng item vào UI
        for (int i = 0; i < count; i++)
        {
            var record = sortedRecords[i];
            GameObject item = Instantiate(leaderboardItemPrefab, contentParent);

            // Tìm các Text trong prefab (theo tên)
            var rankText = item.transform.Find("RankText")?.GetComponent<TextMeshProUGUI>();
            var nameText = item.transform.Find("NameText")?.GetComponent<TextMeshProUGUI>();
            var timeText = item.transform.Find("TimeText")?.GetComponent<TextMeshProUGUI>();

            if (record.Name == playerName)
            {
                rankText.color = Color.green;
                nameText.color = Color.green;
                timeText.color = Color.green;
            }

            if (rankText != null) rankText.text = $"{i + 1}";
            if (nameText != null) nameText.text = record.Name;
            if (timeText != null) timeText.text = FormatTime(record.Time);
        }

       
        var index = sortedRecords.FindIndex(r => r.Name == playerName);
        index += 1; // chuyển từ index (0-based) sang rank (1-based)
        UpdatePlayer(index);
    }

    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        int milliseconds = Mathf.FloorToInt((time * 1000f) % 1000f);
        return $"{minutes:00}:{seconds:00}.{milliseconds:000}";
    }
}
