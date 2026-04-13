using UnityEngine;

public class LoopMap : MonoBehaviour
{
    [Header("Map Objects (2 objects to loop)")]
    public Transform map1;
    public Transform map2;

    [Header("Scroll Settings")]
    public float scrollSpeed = 5f; // tốc độ di chuyển
    public bool moveLeft = true;   // di chuyển qua trái (true) hay phải (false)

    private Vector3 map1StartPos;
    private Vector3 map2StartPos;
    private float mapLength; // độ dài map theo trục Z (hoặc X tùy hướng)

    void Start()
    {
        // Lưu tọa độ ban đầu
        map1StartPos = map1.position;
        map2StartPos = map2.position;

        // Tính khoảng cách giữa 2 map (độ dài mỗi map)
        mapLength = Mathf.Abs(map2StartPos.z - map1StartPos.z);
    }

    void Update()
    {
        // Hướng di chuyển: nếu moveLeft thì giảm trục Z
        float moveDir = moveLeft ? -1f : 1f;
        float moveAmount = scrollSpeed * Time.deltaTime * moveDir;

        // Di chuyển cả 2 map
        map1.position += new Vector3(0, 0, moveAmount);
        map2.position += new Vector3(0, 0, moveAmount);

        // Kiểm tra nếu map2 “đã chạm” vị trí map1 ban đầu
        if (moveLeft)
        {
            if (map2.position.z <= map1StartPos.z)
            {
                // Đưa map1 lên trước map2
                map1.position = new Vector3(map1.position.x, map1.position.y, map2.position.z + mapLength);
                
                // Hoán đổi vị trí lưu trữ để tiếp tục vòng lặp
                SwapMaps();
            }
        }
        else
        {
            if (map2.position.z >= map1StartPos.z)
            {
                map1.position = new Vector3(map1.position.x, map1.position.y, map2.position.z - mapLength);
                SwapMaps();
            }
        }
    }

    void SwapMaps()
    {
        // Đổi chỗ 2 map để vòng lặp logic luôn đúng
        Transform temp = map1;
        map1 = map2;
        map2 = temp;

        // Cập nhật lại tọa độ tham chiếu ban đầu
        map1StartPos = map1.position;
        map2StartPos = map2.position;
    }
}
