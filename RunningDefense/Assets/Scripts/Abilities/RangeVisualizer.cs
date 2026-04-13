using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class RangeVisualizer : MonoBehaviour
{
    public Color baseColor = Color.yellow;
    public Color dangerColor = Color.red;
    public Transform player;
    public float pulseSpeed = 2f;
    public float pulseScale = 0.05f;

    private MeshRenderer meshRenderer;
    private Material mat;
    private Vector3 baseScale;
    public float height;

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        mat = meshRenderer.material;
        baseScale = transform.localScale;

        mat.color = baseColor;
    }

    void Update()
    {
        if (player != null)
        {
            transform.position = player.position + new Vector3(0, height, 0);
        }

        // Pulse effect (scale nhịp tim)
        float pulse = 1 + Mathf.Sin(Time.time * pulseSpeed) * pulseScale;
        transform.localScale = baseScale * pulse;

        // Color lerp effect
        Color c = Color.Lerp(baseColor, dangerColor, (Mathf.Sin(Time.time * pulseSpeed) + 1) / 2f);
        mat.color = c;
    }

    public void SetRadius(float range)
    {
        baseScale = new Vector3(range * 0.2f, 1, range * 0.2f);
        transform.localScale = baseScale;
    }
}
