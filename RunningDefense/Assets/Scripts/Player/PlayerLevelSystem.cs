using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLevelSystem : MonoBehaviour
{
    public PlayerStats playerStats; // Tham chiếu đến PlayerStats để theo dõi level

    public UpgradeUI upgradeUI;      // Tham chiếu UI Popup

    public AudioClip levelUpSFX;    // Hiệu ứng âm thanh khi lên level
    public GameObject levelUpEffect;    // Hiệu ứng âm thanh khi chọn upgrade

    public List<UpgradeOption> allUpgrades = new List<UpgradeOption>();

    private AbilityUpgrade abilityUpgrade; // Tham chiếu đến AbilityUpgrade để áp dụng nâng cấp

    // Hàng chờ các upgrade request
    private List<int> upgradeQueue = new List<int>();
    private bool isShowingUpgrade = false;


    private void OnEnable()
    {
        playerStats.OnLevelUp += TriggerLevelUp;
        upgradeUI.OnUpgradeSelected += OnUpgradeFinished; // callback khi chọn xong
    }

    private void OnDisable()
    {
        playerStats.OnLevelUp -= TriggerLevelUp;
        upgradeUI.OnUpgradeSelected -= OnUpgradeFinished;
    }

    private void Start()
    {
        abilityUpgrade = GetComponent<AbilityUpgrade>();
        InitGame();
        levelUpEffect.SetActive(false);
    }

    public void TriggerLevelUp()
    {
        StartCoroutine(OnLevelUp());
    }
    IEnumerator OnLevelUp()
    {
        SoundManager.Instance.PlaySFX(levelUpSFX);
        if (levelUpEffect != null)
            levelUpEffect.SetActive(true);
            levelUpEffect.GetComponent<ParticleSystem>().Play();

        yield return new WaitForSeconds(1.5f);
        levelUpEffect.SetActive(false);

        List<UpgradeOption> options = GetRandomUpgrades(3);

        // Nếu đang hiển thị upgrade thì cho vào queue
        if (isShowingUpgrade)
        {
            upgradeQueue.Add(1);
        }
        else
        {
            ShowUpgrade(options);
        }
    }
    private void ShowUpgrade(List<UpgradeOption> options)
    {
        Time.timeScale = 0f; // Tạm dừng game khi hiện UI
        isShowingUpgrade = true;
        upgradeUI.ShowOptions(options, abilityUpgrade);
    }

    private void OnUpgradeFinished()
    {
        Time.timeScale = 1f;
        isShowingUpgrade = false;

        // Nếu còn trong queue thì bật tiếp
        if (upgradeQueue.Count > 0)
        {
           upgradeQueue.RemoveAt(0);
          StartCoroutine(OnLevelUp());
        }
    }

    private void InitGame()
    {
        List<UpgradeOption> options = GetRandomUpgrades(3);
        ShowUpgrade(options);
    }
    public List<UpgradeOption> GetRandomUpgrades(int count)
    {
        List<UpgradeOption> pool = new List<UpgradeOption>();

        foreach (var option in allUpgrades)
        {
            // Nếu ability chưa unlock và option này thuộc dạng upgrade -> bỏ qua
            if (!abilityUpgrade.IsUnlocked(option) && !IsUnlockType(option.type))
                continue;

            // Nếu ability đã unlock và option này thuộc dạng unlock -> bỏ qua
            if (abilityUpgrade.IsUnlocked(option) && IsUnlockType(option.type))
                continue;

            option.Init(abilityUpgrade);
            option.DescriptionUpdate();
            pool.Add(option);
        }

        List<UpgradeOption> selected = new List<UpgradeOption>();
        for (int i = 0; i < count && pool.Count > 0; i++)
        {
            int index = UnityEngine.Random.Range(0, pool.Count);
            selected.Add(pool[index]);
            pool.RemoveAt(index);
        }

        return selected;
    }

    private bool IsUnlockType(AbilityUpgradeType type)
    {
        return type == AbilityUpgradeType.MeleeWeaponUnlock
            || type == AbilityUpgradeType.ThrustWeaponUnlock
            || type == AbilityUpgradeType.RangeWeaponUnlock;
    }
}
