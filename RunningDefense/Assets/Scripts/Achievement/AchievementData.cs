using UnityEngine;

[CreateAssetMenu(fileName = "NewAchievement", menuName = "Game/Achievement")]
public class AchievementData : ScriptableObject
{
    public string id;           // ID duy nhất
    public string title;        // Tên hiển thị
    public string description;  // Mô tả
    public AchievementType type;// Loại thành tựu
    public int targetValue;     // Mốc cần đạt (vd: giết 100 quái)
    public int currentValue;    // Giá trị hiện tại (vd: đã giết được 30 quái)
    public int reward;          // Phần thưởng (vd: 50 vàng)
    public bool isClaimed;          // Đã nhận thưởng chưa
}


public enum AchievementType
{
    KillEnemies,    // giết quái
    EliteEnemies, // giết quái elite
    BossEnemies, // giết boss
    MeterMoved,    // di chuyển được bao nhiêu mét
}