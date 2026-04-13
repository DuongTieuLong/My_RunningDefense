using UnityEngine;

[System.Serializable]
public class DifficultyStats
{
    public float healthMultiplier = 1f;
    public float damageMultiplier = 1f;
    public float speedMultiplier = 1f;
}

public class EnemyDifficultyScaler : MonoBehaviour
{
    [Header("Scaling Settings")]
    public float scaleInterval = 60f; // mỗi 60 giây tăng sức mạnh
    public float healthGrowth = 0.2f;
    public float damageGrowth = 0.15f;

    private float lastScaleTime = 0f;
    private DifficultyStats currentScale = new DifficultyStats();

    public DifficultyStats GetCurrentScale() => currentScale;

    public void UpdateDifficulty(float elapsedTime)
    {
        if (elapsedTime - lastScaleTime >= scaleInterval)
        {
            lastScaleTime = elapsedTime;

            currentScale.healthMultiplier += healthGrowth;
            currentScale.damageMultiplier += damageGrowth;
        }
    }
}
