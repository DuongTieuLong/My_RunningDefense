using UnityEngine;
using TMPro;
public class EndGameUI : MonoBehaviour
{
    public TextMeshProUGUI bestTimePlayed;
    public TextMeshProUGUI currentTimePlayed;
    public TextMeshProUGUI totalGoldText;

    public GameObject endGamePanel;

    private float elapsedTime;
    private string playerName;

    private void Start()
    {
        // Giả sử playerName được lưu ở đâu đó (ví dụ qua hệ thống đăng nhập)
        playerName = PlayerPrefs.GetString("PlayerName", "Guest");
        elapsedTime = 0f;
    }

    private void Update()
    {
        UpdateTimer();
    }

    private void UpdateTimer()
    {
        elapsedTime += Time.deltaTime;
    }

    public void ShowEndGameStats()
    {
        endGamePanel.SetActive(true);
        endGamePanel.GetComponent<UIAnimator>().ToggleUIIngoreTimeScale();

        var stats = GameSessionManager.Instance.CurrentStats;
        totalGoldText.text = $"{stats.TotalGoldEarned}";

        SaveAndDisplayPlayTime();
    }

    private void SaveAndDisplayPlayTime()
    {
        // Đọc best time hiện tại của người chơi
        float bestTime = PlayerPrefs.GetFloat($"BestTime_{playerName}", 0f);

        // Nếu chưa có hoặc thời gian hiện tại tốt hơn (nhỏ hơn), thì cập nhật
        if (bestTime == 0f || bestTime < elapsedTime )
        {
            bestTime = elapsedTime;
            PlayerPrefs.SetFloat($"BestTime_{playerName}", bestTime);
            PlayerPrefs.Save();
        }

        // Hiển thị thông tin ra UI
        currentTimePlayed.text = $"{FormatTime(elapsedTime)}";
        bestTimePlayed.text = $"{FormatTime(bestTime)}";
    }

    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        int milliseconds = Mathf.FloorToInt((time * 1000f) % 1000f);
        return $"{minutes:00}:{seconds:00}.{milliseconds:000}";
    }

    public void BackMainScreen()
    {
        Time.timeScale = 1f;
        SceneLoader.Instance.LoadScene("MenuScene");
    }
}
