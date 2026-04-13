using UnityEngine;

[ExecuteAlways] // Cho phép xem ngay trong Editor
public class GhostWall : MonoBehaviour
{
    [Header("Wall Prefab (Particle Plane)")]
    public GameObject wallPrefab;

    [Header("Movement Bounds")]
    public float minX = -30f;
    public float maxX = 28f;
    public float minZ = -30f;
    public float maxZ = 28f;

    [Header("Wall Settings")]
    public float wallHeight = 5f;
    public float wallYPos = 0.5f;

    [Tooltip("Điều chỉnh độ dài thực tế nếu prefab plane không phải tỉ lệ 1:1")]
    public float lengthScaleFactor = 0.1f; // <--- GIẢM ĐỘ DÀI (anh có thể chỉnh thử 0.1, 0.2,...)

    void OnEnable() => SpawnWalls();

/*#if UNITY_EDITOR
    void OnValidate()
    {
        if (!Application.isPlaying)
            SpawnWalls();
    }
#endif*/

    void ClearOldWalls()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
            DestroyImmediate(transform.GetChild(i).gameObject);
    }

    void SpawnWalls()
    {
        if (wallPrefab == null) return;
        ClearOldWalls();

        float width = (maxX - minX) * lengthScaleFactor;
        float depth = (maxZ - minZ) * lengthScaleFactor;

        // Nam (-Z)
        CreateWall(new Vector3(0, wallYPos, minZ), 0f, width);
        // Bắc (+Z)
        CreateWall(new Vector3(0, wallYPos, maxZ), 180f, width);
        // Tây (-X)
        CreateWall(new Vector3(minX, wallYPos, 0), 90f, depth);
        // Đông (+X)
        CreateWall(new Vector3(maxX, wallYPos, 0), -90f, depth);
    }

    void CreateWall(Vector3 pos, float rotY, float scaleX)
    {
        GameObject wall = Instantiate(wallPrefab, pos, Quaternion.Euler(0, rotY, 0), transform);
        wall.transform.localScale = new Vector3(scaleX, wallHeight, 1f);
        wall.name = $"Wall_{rotY}";
    }
}
