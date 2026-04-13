using UnityEngine;

public class PlayerMovementTracker : MonoBehaviour
{
    private Vector3 lastPosition;
    private float distanceThisRun; 

    public float DistanceThisRun => distanceThisRun;

    private void Start()
    {
        lastPosition = transform.position;
        distanceThisRun = 0f;
    }

    float SinceLastSubmit = 1f;
    private void Update()
    {
        float distance = Vector3.Distance(transform.position, lastPosition);

        if (distance > 0.001f) 
        {
            distanceThisRun += distance;
        }

        lastPosition = transform.position;

        if(Time.time > SinceLastSubmit)
        {
            SubmitDistanceToAchievement();
                SinceLastSubmit = Time.time + 1f;

        }
    }



    public void SubmitDistanceToAchievement()
    {
        int meters = Mathf.RoundToInt(distanceThisRun);

        if(AchievementManager.Instance == null) 
        {
            return;
        }
        AchievementManager.Instance.AddProgress(AchievementType.MeterMoved, meters);


        distanceThisRun = 0f;
    }
}
