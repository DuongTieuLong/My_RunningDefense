using UnityEngine;
using System;
using System.Collections;

public abstract class Enemy : MonoBehaviour
{
    [Header("Stats")]
    public EnemyRank rank = EnemyRank.normal;
    public float maxHealth = 50f;
    public float moveSpeed = 2f;
    public float attackDamage = 10f;
    public float expDrop = 10;
    public float attackRate = 1f;
    public int coinReward = 3; // số coin nhận được khi kill enemy

    float attackDelay;

    [Header("AI Settings")]
    [Tooltip("Bán kính mà enemy có thể phát hiện player")]
    public float detectionRange = 10f;
    [SerializeField] private float separationRadius = 2f;

    // weight khi cộng vector separation vào movement (tăng để tránh dính mạnh hơn)
    [Tooltip("Trọng số cho lực tách (separation) khi tránh chồng lấp")]
    [SerializeField] private float separationWeight = 1.2f;

    [SerializeField] protected float currentHealth;
    protected float lastAttackTime;
    private Action onReturnToPool;

    protected Transform player;
    protected Transform tower;
    protected Animator animator;

    private GameObject lastAttacker;
    public static event Action<GameObject, float> OnEnemyKilled;

    protected readonly int hashIsMoving = Animator.StringToHash("isMoving");
    protected readonly int hashAttack = Animator.StringToHash("Attack");
    protected readonly int hashDie = Animator.StringToHash("Die");

    public bool isDead = false;

    public virtual void Init(Action onReturnToPool)
    {
        this.onReturnToPool = onReturnToPool;
        currentHealth = maxHealth;
        lastAttackTime = -attackRate;
        isDead = false;

        // Lấy lại player mỗi lần init để đảm bảo tham chiếu luôn mới
        player = EnemyManager.Instance.GetPlayer();
        tower = EnemyManager.Instance.GetTower();

        EnemyManager.Instance.RegisterEnemy(this);

        if (!animator) animator = GetComponent<Animator>();
        ResetState();
    }


    protected virtual void ResetState()
    {
        transform.rotation = Quaternion.identity;
        if (animator)
        {
            animator.SetBool(hashIsMoving, false);
            animator.ResetTrigger(hashAttack);
            animator.Rebind();
        }
    }

    public abstract void CustomUpdate();

    protected void LookAtTarget(Vector3 dir, float rotateSpeed = 10f)
    {
        if (dir.sqrMagnitude > 0.01f)
        {
            Quaternion lookRot = Quaternion.LookRotation(dir.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * rotateSpeed);
        }
    }

    public void TakeDamage(float dmg, GameObject attacker = null)
    {
        if (isDead) return;
        currentHealth -= dmg;

        var s = GetComponentInChildren<ShowTakeDamage>();
        if (s != null) s.ShowDamage(dmg);


        if (attacker != null) lastAttacker = attacker;
        if (currentHealth <= 0) Die();
    }

    protected void Die()
    {
        if (isDead) return;
        isDead = true;

        OnEnemyKilled?.Invoke(lastAttacker, expDrop);

        if (lastAttacker == player?.gameObject)
        {
            if (rank == EnemyRank.elite) AchievementManager.Instance.AddProgress(AchievementType.EliteEnemies, 1);
            else if (rank == EnemyRank.boss) AchievementManager.Instance.AddProgress(AchievementType.BossEnemies, 1);
            else AchievementManager.Instance.AddProgress(AchievementType.KillEnemies, 1);

            // cộng coin cho người chơi thông qua CoinManager (nếu có)
            if (CoinManager.Instance != null)
                CoinManager.Instance.AddCoin(coinReward);
            GameSessionManager.Instance.OnEnemyKilled(rank, coinReward);
        }

        if (animator)
        {
            animator.SetTrigger(hashDie);
            EnemyManager.Instance.StartCoroutine(ReturnAfterDelay(1.5f));
        }
        else
        {
            EnemyManager.Instance.UnregisterEnemy(this);
            onReturnToPool?.Invoke();
        }
    }

    private IEnumerator ReturnAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        EnemyManager.Instance.UnregisterEnemy(this);
        onReturnToPool?.Invoke();
    }

    protected Transform GetPriorityTarget()
    {
        // Kiểm tra xem player và tower có còn hoạt động trong scene không
        bool playerValid = player != null && player.gameObject.activeInHierarchy;
        bool towerValid = tower != null && tower.gameObject.activeInHierarchy;

        // Ưu tiên player nếu trong tầm phát hiện
        if (playerValid)
        {
            float distToPlayer = Vector3.Distance(transform.position, player.position);
            if (distToPlayer <= detectionRange)
                return player;
        }

        // Nếu player không hợp lệ hoặc ở xa, thì dùng tower nếu có
        if (towerValid)
            return tower;

        // Nếu cả hai đều chết hoặc bị disable → không có target
        return null;
    }


    protected Vector3 SeparateFromOthers(float separationForce = 1f)
    {
        // Chỉ tính trên mặt phẳng XZ (không thay đổi Y)
        Vector3 separation = Vector3.zero;
        int count = 0;
        Vector3 pos = transform.position;
        foreach (var other in EnemyManager.Instance.ActiveEnemies)
        {
            if (other == this) continue;
            Vector3 otherPos = other.transform.position;

            // khoảng cách trên mặt phẳng XZ
            Vector2 a = new Vector2(pos.x, pos.z);
            Vector2 b = new Vector2(otherPos.x, otherPos.z);
            float dist = Vector2.Distance(a, b);
            if (dist < 0.001f) continue;

            if (dist < separationRadius)
            {
                // vector từ other -> this trên XZ
                Vector3 diff = new Vector3(pos.x - otherPos.x, 0f, pos.z - otherPos.z);
                // nếu trùng hợp hoàn toàn thì sinh vector ngẫu nhiên nhỏ để tách
                if (diff.sqrMagnitude < 0.0001f)
                    diff = new Vector3(UnityEngine.Random.Range(-0.01f, 0.01f), 0f, UnityEngine.Random.Range(-0.01f, 0.01f));

                separation += diff.normalized / dist; // tỷ lệ theo khoảng cách
                count++;
            }
        }
        if (count > 0)
            separation = (separation / count) * separationForce;

        return separation;
    }

    protected void MoveTowards(Vector3 dir, float speed = -1f)
    {
        if (speed < 0f) speed = moveSpeed;
        if (isDead) return;

        // lấy hướng chỉ trên XZ
        Vector3 horizontalDir = new Vector3(dir.x, 0f, dir.z);
        if (horizontalDir.sqrMagnitude <= 0.0001f) return;

        // lưu y hiện tại để không thay đổi trục Y
        float currentY = transform.position.y;

        // lấy vector separation (trên XZ) và kết hợp với hướng di chuyển
        Vector3 sep = SeparateFromOthers(separationWeight);
        Vector3 combined = horizontalDir.normalized + sep;
        // nếu combined gần bằng 0 (bị đè ngược) fallback về horizontalDir
        if (combined.sqrMagnitude <= 0.0001f)
            combined = horizontalDir.normalized;

        Vector3 move = combined.normalized * speed * Time.deltaTime;

        // cập nhật vị trí (không thay đổi y)
        Vector3 newPos = transform.position + new Vector3(move.x, 0f, move.z);
        newPos.y = currentY;
        transform.position = newPos;
    }

    protected void PlayMoveAnim(bool moving) { if (animator && !isDead) animator.SetBool(hashIsMoving, moving); }
    protected void PlayAttackAnim() { if (animator && !isDead) animator.SetTrigger(hashAttack); }
}

public enum EnemyRank { normal, elite, boss }