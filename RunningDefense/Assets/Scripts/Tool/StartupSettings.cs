using UnityEngine;

public class StartupSettings : MonoBehaviour
{
    public GameObject tutorial;
    public GameObject nameInput;
    void Awake()
    {
        Application.targetFrameRate = 60; 
        QualitySettings.vSyncCount = 0;   // không giới hạn FPS
    }


    public void Start()
    {
        Tutorial();
        PlyerName();
    }

    public void Tutorial()
    {
        if (PlayerPrefs.GetInt("FirstLaunch", 0) == 0)
        {
            tutorial.GetComponent<UIAnimator>().ToggleUI();
            PlayerPrefs.SetInt("FirstLaunch", 1);
        }
        else
        {
            tutorial.SetActive(false);
        }
    }
    public void PlyerName()
    {
        var playerName = PlayerPrefs.GetString("PlayerName", "");
        if (string.IsNullOrEmpty(playerName))
        {
            nameInput.SetActive(true);
        }
        else
        {
            nameInput.SetActive(false);
        }
    }
    [ContextMenu("ResetTutorial")]
    public void ResetTutorial()
    {
        PlayerPrefs.SetInt("FirstLaunch", 0);
    }
}
