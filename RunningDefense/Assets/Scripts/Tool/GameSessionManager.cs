using System;
using UnityEngine;

public class GameSessionManager : MonoBehaviour
{
    public static GameSessionManager Instance { get; private set; }
    public MatchStats CurrentStats { get; private set; }

    public EndGameUI endGameUI;
    public AudioClip losingSFX;
    public bool playLosingMusic = false;
    public bool EndGameTriggered { get; private set; } = false;

    public GameObject player;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            CurrentStats = new MatchStats();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void OnEnemyKilled(EnemyRank type, int goldReward)
    {
        CurrentStats.AddKill(type, goldReward);
    }

    public void EndGame()
    {
        player.GetComponent<PlayerRespawnManager>().CancelDeathSequence();
        EndGameTriggered = true;
        SoundManager.Instance.StopBGM();
        if (playLosingMusic)
            SoundManager.Instance.PlayLosingBGM();
        else
            SoundManager.Instance.PlaySFX(losingSFX);
        endGameUI.ShowEndGameStats();
        Time.timeScale = 0f; // Tạm dừng games
    }
}
