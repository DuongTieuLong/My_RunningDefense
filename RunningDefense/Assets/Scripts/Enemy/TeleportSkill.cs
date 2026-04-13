using DG.Tweening;
using UnityEngine;

public class TeleportSkill : MonoBehaviour
{
    public float teleportCooldown = 8f;
    public float teleportDistanceBehind = 2f;
    public AudioClip teleportSFX;

    private float lastTeleportTime;
    private Transform player;
    private Animator animator;

    public ParticleSystem teleportVFX;

    void Start()
    {
        player = EnemyManager.Instance.GetPlayer();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (player == null) return;

        if (Time.time - lastTeleportTime > teleportCooldown)
        {
            lastTeleportTime = Time.time;
            TeleportBehindPlayer();
        }
    }
    [SerializeField] private float vfxMoveDuration = 0.5f;

    void TeleportBehindPlayer()
    {
        Vector3 oldPos = transform.position;
        Vector3 newPos = player.position - player.forward * teleportDistanceBehind;

        transform.position = newPos;

        if (teleportVFX)
        {
            teleportVFX.transform.position = oldPos;
            teleportVFX.gameObject.SetActive(true);
            teleportVFX.Play();
            teleportVFX.transform.DOMove(newPos, vfxMoveDuration).SetEase(Ease.OutQuad)
                .OnComplete(() =>
                {
                    teleportVFX.transform.position = newPos;
                });
        }


        if (teleportSFX)
            SoundManager.Instance.PlaySFX(teleportSFX);

        if (animator)
            animator.SetTrigger("Teleport");

        Invoke(nameof(EndEffect), 0.5f);
    }

    public void EndEffect()
    {
        if (teleportVFX)
            teleportVFX.Stop();
    }
}
