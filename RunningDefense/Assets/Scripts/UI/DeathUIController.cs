using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using TMPro;

public class DeathUIController : MonoBehaviour
{
    [SerializeField] private CanvasGroup deathPanel;
    [SerializeField] private TextMeshProUGUI countdownText;

    public async UniTask ShowDeathUI()
    {
        deathPanel.gameObject.SetActive(true);
        deathPanel.alpha = 0;
        await deathPanel.DOFade(1, 1f).AsyncWaitForCompletion();
    }

    public void UppdateCountDown(int secondsLeft)
    {
        if (countdownText != null)
        {
            countdownText.text = $"{secondsLeft}";
        }
    }

    public async UniTask HideDeathUI()
    {
        await deathPanel.DOFade(0, 0.3f).AsyncWaitForCompletion();
        if(deathPanel != null)
            deathPanel.gameObject.SetActive(false);
    }
}
