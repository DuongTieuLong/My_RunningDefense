using System;
using System.IO;
using UnityEngine;

public class CoinManager : MonoBehaviour
{
    public static CoinManager Instance { get; private set; }

    public event Action<int> OnCoinChanged; // Gọi khi số coin thay đổi

    [SerializeField] private int coin;
    private string savePath => Path.Combine(Application.persistentDataPath, "coin_save.json");

    [SerializeField] private bool autoSaveOnChange = true;

    private void Awake()
    {
        // Singleton setup
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        Load();
    }

    // ================== API ==================
    public int GetCoin() => coin;



    public void SetCoin(int amount)
    {
        coin = Mathf.Max(0, amount);
        OnCoinChanged?.Invoke(coin);
        if (autoSaveOnChange) Save();
    }

    public void AddCoin(int amount)
    {
        if (amount <= 0) return;
        coin += amount;
        OnCoinChanged?.Invoke(coin);
        if (autoSaveOnChange) Save();
    }

    public bool TrySpend(int amount)
    {
        if (amount <= 0) return false;
        if (coin < amount) return false;

        coin -= amount;
        OnCoinChanged?.Invoke(coin);
        if (autoSaveOnChange) Save();
        return true;
    }

    [ContextMenu("Reset Coin")]
    public void ResetCoin()
    {
        SetCoin(0);
        OnCoinChanged?.Invoke(coin);
        if (File.Exists(savePath)) File.Delete(savePath);
    }

    // ================== SAVE / LOAD ==================

    [Serializable]
    private class CoinData { public int coin; }

    public void Save()
    {
        try
        {
            var data = new CoinData { coin = this.coin };
            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(savePath, json);
        }
        catch (Exception e)
        {
            Debug.Log($"Save coin failed: {e}");
        }
    }

    public void Load()
    {
        try
        {
            if (!File.Exists(savePath))
            {
                return;
            }

            string json = File.ReadAllText(savePath);
            var data = JsonUtility.FromJson<CoinData>(json);
            coin = data.coin;
        }
        catch (Exception e)
        {
            Debug.Log($"Load coin failed: {e}");
        }

        OnCoinChanged?.Invoke(coin);
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause) Save();
    }

    private void OnApplicationQuit()
    {
        Save();
    }
}
