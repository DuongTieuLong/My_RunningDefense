using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class TurretRangeDrawer : MonoBehaviour
{
    public int segments = 200; // độ mịn vòng tròn
    public float lineWidth = 0.5f;
    public Color lineColor = Color.green;

    public LineRenderer line;

    void Start()
    {
        line.useWorldSpace = false;
        line.loop = true;
        line.positionCount = segments;
        line.startWidth = lineWidth;
        line.endWidth = lineWidth;
        line.material = new Material(Shader.Find("Sprites/Default"));
        line.startColor = lineColor;
        line.endColor = lineColor;

    }

    float radius;
    public void DrawCircle(float baseRadius)
    {
         radius = baseRadius/2;
        if (line == null) return;

        line.positionCount = segments + 1; // 🔥 quan trọng

        float angle = 0f;

        for (int i = 0; i <= segments; i++)
        {
            float x = Mathf.Sin(Mathf.Deg2Rad * angle) * radius;
            float z = Mathf.Cos(Mathf.Deg2Rad * angle) * radius;
            line.SetPosition(i, new Vector3(x, 0.1f, z));
            angle += 360f / segments;
        }
    }
}
