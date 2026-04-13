using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class AchievementUIManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject achievementPrefab; // Prefab 1 item
    public Transform contentParent;      // Nơi chứa danh sách achievement (vd: ScrollView Content)

    [Header("Data")]
    public List<AchievementData> achievements;

    private List<GameObject> spawnedItems = new List<GameObject>();

    UnityEngine.Events.UnityAction onClickAction;

    private void Start()
    {
        achievements = AchievementManager.Instance.achievements;
        ShowAllAchievements();
    }

    public void ShowAllAchievements()
    {
        // Xóa UI cũ
        foreach (var go in spawnedItems) Destroy(go);
        spawnedItems.Clear();

        // Tạo mới UI cho từng achievement
        foreach (var achievement in achievements)
        {

            GameObject item = Instantiate(achievementPrefab, contentParent);
            spawnedItems.Add(item);

            // Lấy các component từ prefab
            TextMeshProUGUI titleText = item.transform.Find("TitleText").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI descriptionText = item.transform.Find("DescriptionText").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI progressText = item.transform.Find("ProgressText").GetComponent<TextMeshProUGUI>();
            Button claimButton = item.transform.Find("ClaimButton").GetComponent<Button>();
            TextMeshProUGUI rewardText = claimButton.transform.Find("RewardText").GetComponent<TextMeshProUGUI>();
            GameObject claimedMark = claimButton.transform.Find("ClaimedMark").gameObject;

            // Gán dữ liệu
            titleText.text = achievement.title;
            descriptionText.text = achievement.description;
            progressText.text = $"{achievement.currentValue}/{achievement.targetValue}";
            rewardText.text = $"{achievement.reward}";


            // Trạng thái nút nhận  
            bool canClaim = !achievement.isClaimed && achievement.currentValue >= achievement.targetValue;
            claimButton.interactable = (!achievement.isClaimed && canClaim);
            claimedMark.SetActive(achievement.isClaimed && !canClaim);

            // Xử lý nút nhận
            SetupButton(claimButton, achievement, claimedMark);
        }
    }
    void SetupButton(Button claimButton, AchievementData achievement, GameObject claimedMark)
    {
        // Nếu trước đó đã add -> remove
        if (onClickAction != null)
        {
            claimButton.onClick.RemoveListener(onClickAction);
            onClickAction = null;
        }


        onClickAction = () => ClaimReward(achievement, claimButton, claimedMark);
        claimButton.onClick.AddListener(onClickAction);
    }

    private void ClaimReward(AchievementData achievement, Button claimButton, GameObject claimedMark)
    {
        AchievementManager.Instance.Claimed(achievement);
         CoinManager.Instance.AddCoin(achievement.reward);

        // Cập nhật UI
        claimButton.interactable = false;
        claimedMark.SetActive(true);
    }
}
