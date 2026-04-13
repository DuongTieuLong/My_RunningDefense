using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class PlayerMove : MonoBehaviour
{
    [Header("References")]
    [SerializeField] public Rigidbody rb;
    [SerializeField] private FixedJoystick joystick;
    [SerializeField] private Button dodgeButton;
    public Image dodgeCooldownImage;
    [SerializeField] private PlayerStats playerStats;
    private Animator animator;

    [Header("Move Settings")]
    public float moveSpeedMultiplier = 1f;
    public Vector3 velocity = Vector3.zero;

    [Header("Dash Settings")]
    public float dashDistance = 5f;
    public float dashDuration = 0.2f;
    public float dodgeTime = 0.3f;
    private bool canDodge = true;

    [Header("PC Test Controls")]
    public bool enableKeyboardControl = true;
    public KeyCode dashKey = KeyCode.Space;

    // Giới hạn vùng di chuyển (x: -30 -> 28, z: -30 -> 28)
    private const float minX = -30f;
    private const float maxX = 30f;
    private const float minZ = -30f;
    private const float maxZ = 30f;

    private void Awake()
    {
        playerStats = GetComponent<PlayerStats>();
        animator = GetComponent<Animator>();
        if (rb == null) rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        if (dodgeButton != null)
        {
            dodgeButton.onClick.AddListener(() =>
            {
                if (canDodge)
                    StartCoroutine(Dash(dashDistance, dashDuration));
            });
        }
    }

    private void Update()
    {
        Vector3 input = Vector3.zero;

        if (joystick != null && (joystick.Horizontal != 0 || joystick.Vertical != 0))
        {
            input = new Vector3(joystick.Horizontal, 0, joystick.Vertical);
        }

        if (enableKeyboardControl)
        {
            float h = Input.GetAxisRaw("Horizontal"); 
            float v = Input.GetAxisRaw("Vertical");  
            if (Mathf.Abs(h) > 0.01f || Mathf.Abs(v) > 0.01f)
                input = new Vector3(h, 0, v);
        }

        velocity = input.normalized * playerStats.moveSpeed * moveSpeedMultiplier;
        animator.SetFloat("Speed", input.magnitude);

        if (enableKeyboardControl && canDodge && Input.GetKeyDown(dashKey))
        {
            StartCoroutine(Dash(dashDistance, dashDuration));
        }
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector3(velocity.x, rb.linearVelocity.y, velocity.z);

        if (velocity.sqrMagnitude > 0.01f)
        {
            transform.rotation = Quaternion.LookRotation(new Vector3(velocity.x, 0, velocity.z));
        }

        // đảm bảo vị trí luôn nằm trong vùng giới hạn
        ApplyPositionClamp();
    }

    private IEnumerator Dash(float distance, float duration)
    {
        canDodge = false;
        playerStats.DodgeTime = true;
        animator.SetBool("Dodge", true);
        Invoke(nameof(ResetDodgeTime), dodgeTime);

        Vector3 start = GetClampedPosition(transform.position);
        Vector3 end = start + transform.forward * distance;
        end = GetClampedPosition(end); // không cho dash vượt ra ngoài

        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            rb.MovePosition(Vector3.Lerp(start, end, t));
            // sau mỗi bước di chuyển, đảm bảo clamp
            ApplyPositionClamp();
            yield return null;
        }

        // Cooldown
        float cd = playerStats.dogdeCooldown;
        dodgeCooldownImage.fillAmount = 1f;

        while (cd > 0f)
        {
            cd -= Time.deltaTime;
            dodgeCooldownImage.fillAmount = cd / playerStats.dogdeCooldown;
            yield return null;
        }

        dodgeCooldownImage.fillAmount = 0f;
        canDodge = true;
    }

    private void ResetDodgeTime()
    {
        playerStats.DodgeTime = false;
        animator.SetBool("Dodge", false);
    }

    // Trả về vị trí đã được clamp theo giới hạn x/z
    private Vector3 GetClampedPosition(Vector3 pos)
    {
        float x = Mathf.Clamp(pos.x, minX, maxX);
        float z = Mathf.Clamp(pos.z, minZ, maxZ);
        return new Vector3(x, pos.y, z);
    }

    // Áp dụng clamp lên Rigidbody / Transform nếu cần
    private void ApplyPositionClamp()
    {
        Vector3 clamped = GetClampedPosition(transform.position);
        if (clamped.x != transform.position.x || clamped.z != transform.position.z)
        {
            // đặt trực tiếp lên Rigidbody để tránh xung đột với physics
            rb.position = clamped;
            // đồng bộ transform (rb.position đã làm việc nhưng đảm bảo)
            transform.position = clamped;
        }
    }
}
