using UnityEngine;
using System.Collections;

public class JumpSmashSkill : MonoBehaviour
{
    public float jumpCooldown = 10f;
    public float jumpHeight = 3f;
    public float aoeRadius = 6f;
    public float damageMultiplier = 3f;
    public AudioClip smashSFX;
    public ParticleSystem smashEffect;

    private float lastJumpTime;
    private Transform player;
    private Enemy enemy;

    // --- ring visualization settings ---
    [Header("Smash Ring Visual")]
    public Material ringMaterial;         // optional, nếu null sẽ dùng Unlit/Color
    public Color ringColor = new Color(1f, 0.2f, 0.2f, 1f);
    public int ringSegments = 64;
    public float ringWidth = 0.15f;
    public float ringDuration = 0.9f;

    void Start()
    {
        player = EnemyManager.Instance.GetPlayer();
        enemy = GetComponent<Enemy>();
    }

    void Update()
    {
        if (player == null) return;

        if (Time.time - lastJumpTime > jumpCooldown)
        {
            lastJumpTime = Time.time;
            StartCoroutine(PerformJumpSmash());
        }
    }

    IEnumerator PerformJumpSmash()
    {
        Vector3 start = transform.position;
        Vector3 apex = start + Vector3.up * jumpHeight;
        Vector3 targetPos = player.position;

        float t = 0f;

        // bay lên
        while (t < 1f)
        {
            transform.position = Vector3.Lerp(start, apex, t);
            t += Time.deltaTime * 2f;
            yield return null;
        }

        // rơi xuống
        t = 0f;
        while (t < 1f)
        {
            transform.position = Vector3.Lerp(apex, targetPos, t);
            t += Time.deltaTime * 3f;
            yield return null;
        }

        // tạo hiệu ứng
        if (smashEffect)
            Instantiate(smashEffect, transform.position, Quaternion.identity);

        if (smashSFX)
            SoundManager.Instance.PlaySFX(smashSFX);

        // vẽ vòng hit box (visual) ngay khi va chạm
        //StartCoroutine(SpawnHitRing(transform.position, aoeRadius, ringDuration));

        // gây damage AOE
        Collider[] hits = Physics.OverlapSphere(transform.position, aoeRadius);
        foreach (var h in hits)
        {
            if (h.TryGetComponent<PlayerStats>(out var p))
                p.TakeDamge(enemy.attackDamage * damageMultiplier);
        }
    }

    // Coroutine tạo vòng hiển thị bằng LineRenderer và fade out
    IEnumerator SpawnHitRing(Vector3 center, float radius, float duration)
    {
        GameObject go = new GameObject("SmashRing");
        go.transform.position = center;
        // đặt ring hơi trên mặt đất để nhìn rõ
        go.transform.position = new Vector3(center.x, center.y + 0.05f, center.z);

        LineRenderer lr = go.AddComponent<LineRenderer>();
        lr.loop = true;
        lr.positionCount = ringSegments;
        lr.useWorldSpace = true;
        lr.startWidth = ringWidth;
        lr.endWidth = ringWidth;
        lr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        lr.receiveShadows = false;
        lr.textureMode = LineTextureMode.Tile;

        Material mat = null;
        if (ringMaterial != null)
            mat = new Material(ringMaterial);
        else
        {
            Shader s = Shader.Find("Unlit/Color");
            mat = (s != null) ? new Material(s) : new Material(Shader.Find("Sprites/Default"));
        }
        lr.material = mat;

        // set positions
        for (int i = 0; i < ringSegments; i++)
        {
            float ang = (float)i / ringSegments * Mathf.PI * 2f;
            float x = Mathf.Cos(ang) * radius;
            float z = Mathf.Sin(ang) * radius;
            lr.SetPosition(i, new Vector3(center.x + x, center.y + 0.05f, center.z + z));
        }

        float elapsed = 0f;
        Color c = ringColor;
        while (elapsed < duration)
        {
            float a = Mathf.Lerp(1f, 0f, elapsed / duration);
            Color cc = new Color(c.r, c.g, c.b, c.a * a);
            if (lr.material != null)
                lr.material.color = cc;
            lr.startColor = cc;
            lr.endColor = cc;

            elapsed += Time.deltaTime;
            yield return null;
        }

        Destroy(go);
    }
}
