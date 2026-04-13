using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.LowLevel;
public class PlayerRespawnManager : MonoBehaviour
{
    [SerializeField] private Transform respawnPoint;
    [SerializeField] private DeathUIController deathUI;
    [SerializeField] private PlayerStats playerHealth;
    [SerializeField] private GameObject playerModel;
    [SerializeField] private GameObject abilitiesOBJ;


    [SerializeField] private int respawnDelay = 5;

    private bool isProcessingDeath = false;

    private void OnEnable()
    {
        playerHealth.OnDeath += StartDeath;
    }

    private void OnDisable()
    {
        playerHealth.OnDeath -= StartDeath;
    }

    private void Start()
    {
       abilitiesOBJ = playerModel.transform.Find("PlayerAbilities").gameObject;
    }

    private CancellationTokenSource deathCts;
    public void StartDeath()
    {
        CancelDeath(); // hủy cũ nếu có
        deathCts = new CancellationTokenSource();
        HandleDeath(deathCts.Token).Forget(); // chạy nền nhưng an toàn
    }

    public void CancelDeath()
    {
        if (deathCts != null)
        {
            deathCts.Cancel();
            deathCts.Dispose();
            deathCts = null;
        }
        isProcessingDeath = false;
    }
    private async UniTask HandleDeath(CancellationToken token)
    {
        await UniTask.Yield(); // đảm bảo ability info đã được cập nhật

        try
        {

            if (isProcessingDeath) return;
            isProcessingDeath = true;


            DisablePlayerControl();

            await UniTask.Delay(1000, cancellationToken: token);
            token.ThrowIfCancellationRequested();

            deathUI.UppdateCountDown(respawnDelay);
            deathUI.ShowDeathUI().Forget();
            DisablePlayer();

            for (int i = respawnDelay; i > 0; i--)
            {
                deathUI.UppdateCountDown(i);
                await UniTask.Delay(1000, cancellationToken: token);
                token.ThrowIfCancellationRequested(); 
            }

            await deathUI.HideDeathUI().AttachExternalCancellation(token);
            token.ThrowIfCancellationRequested();

            Respawn(); 
        }
        catch (OperationCanceledException)
        {
          
        }
        finally
        {
            isProcessingDeath = false;
        }
    }

    public void CancelDeathSequence()
    {
        if (deathCts != null)
        {
            deathCts.Cancel();
            deathCts.Dispose();
            deathCts = null;
        }
        isProcessingDeath = false;  
    }

    private void DisablePlayer()
    {
        if (playerModel != null)
            playerModel.SetActive(false);
    }

    public void DisablePlayerControl()
    {
       playerHealth.GetComponent<PlayerLevelSystem>().enabled = false;
       abilitiesOBJ.SetActive(false);
    }

    private void Respawn()
    {

        transform.position = respawnPoint.position;
        playerModel.SetActive(true);
        playerHealth.GetComponent<PlayerLevelSystem>().enabled = true;
        abilitiesOBJ.SetActive(true);
        playerHealth.RespawnHealth();
    }
}
