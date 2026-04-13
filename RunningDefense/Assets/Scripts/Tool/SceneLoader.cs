using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Giữ lại object này khi load scene mới
        }
        else
        {
            Destroy(gameObject); // Hủy nếu đã có instance khác
        }
    }
    // Load scene theo tên
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
        if(LoadingGamePanel.Instance != null)
            LoadingGamePanel.Instance.StartLoading();
    }

    // Thoát game
    public void QuitGame()
    {
        Application.Quit();
    }
}
