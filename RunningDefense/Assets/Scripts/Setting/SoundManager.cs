using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("Audio Sources")]
    public AudioSource bgmSource;
    public AudioSource sfxSource;

    [Header("SFX Settings")]
    public AudioClip buttonClickSFX;

    [Header("BGM Playlist")]
    [SerializeField] private List<AudioClip> bgmInGamePlaylist;
    [SerializeField] private List<AudioClip> bgmMainMenuPlaylist;
    public AudioClip losingPlay;
    private int currentBgmIndex = 0;
    private int currentMainBgmIndex = 0;

    public bool isGameScene
    {
        get
        {
            var activeScene = SceneManager.GetActiveScene();
            return activeScene.name == "GameScene";
        }
    }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StopBGM();
        StopAllCoroutines();
        StartCoroutine(CheckMusicPlay());
    }


    IEnumerator CheckMusicPlay()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            if (!bgmSource.isPlaying)
            {
                if (isGameScene)
                    PlayNextBGMInGame();
                else
                    PlayNextBGMMainMenu();
            }
        }
    }

    // --- BGM ---
    public void PlayNextBGMInGame()
    {
        if (bgmInGamePlaylist.Count == 0) return;

        bgmSource.clip = bgmInGamePlaylist[currentBgmIndex];
        bgmSource.loop = false; // không loop 1 bài
        bgmSource.Play();

        currentBgmIndex = (currentBgmIndex + 1) % bgmInGamePlaylist.Count; // chuyển sang bài kế
    }

    public void PlayNextBGMMainMenu()
    {
        if (bgmMainMenuPlaylist.Count == 0) return;

        bgmSource.clip = bgmMainMenuPlaylist[currentMainBgmIndex];
        bgmSource.loop = false; // không loop 1 bài
        bgmSource.Play();

        currentMainBgmIndex = (currentMainBgmIndex + 1) % bgmMainMenuPlaylist.Count; // chuyển sang bài kế
    }

    public void PlayLosingBGM()
    {
        if (losingPlay == null) return;
        bgmSource.clip = losingPlay;
        bgmSource.loop = true; // loop bài này
        bgmSource.Play();
    }

    public void StopBGM()
    {
        bgmSource.Stop();
    }

    // --- SFX ---
    public void PlayButtonSFX()
    {
        if (buttonClickSFX != null)
        {
            PlaySFX(buttonClickSFX);
        }
    }
    public void PlaySFX(AudioClip clip, float volume = 1f)
    {
        sfxSource.PlayOneShot(clip, volume);
    }


    public void SetBGMVolume(float volume)
    {
        bgmSource.volume = volume;
    }

    public void SetSFXVolume(float volume)
    {
        sfxSource.volume = volume;
    }

}
