using UnityEngine;
using System.Collections;
using Unit;

namespace Enemy
{
    public class EnemyController : MonoBehaviour
    {
        [Header("Stats")]
        [SerializeField] private int maxHP = 100;
        [SerializeField] private float moveSpeed = 2f;
        [SerializeField] private int goldReward = 10;
        [SerializeField] private int damageToBase = 1;

        [Header("Waypoints")]
        [SerializeField] private Transform[] waypoints;
        [SerializeField] private float waypointReachDistance = 0.1f;

        [Header("Combat AI")]
        [SerializeField] private float detectionRange = 4f;
        [SerializeField] private float attackRange = 1.2f;
        [SerializeField] private float attackCooldown = 1.2f;
        [SerializeField] private int attackDamage = 15;
        [SerializeField] private bool prioritizePlayer = true; // Ưu tiên tấn công nhân vật chính
        [SerializeField] private float maxChaseOffPathDistance = 4f; // Kẻ địch bỏ target nếu target đi xa khỏi đường này

        [Header("Visual Effects")]
        [SerializeField] private GameObject deathEffectPrefab;
        [SerializeField] private Color damageFlashColor = Color.red;
        [SerializeField] private float damageFlashDuration = 0.1f;

        private int currentHP;
        private int currentWaypointIndex = 0;
        private bool isDead = false;
        private bool hasReachedEnd = false;
        private float lastAttackTime = -999f;

        // Combat targets
        private UnitCombat currentUnitTarget;
        private PlayerController currentPlayerTarget;

        private Animator animator;
        private EnemyHPBar hpBar;
        private SpriteRenderer spriteRenderer;
        private Color originalColor;

        // REQUIRED BY COMBAT RULES
        public bool IsAlive => !isDead;

        private void Awake()
        {
            // Init HP
            currentHP = maxHP;

            // Get components
            animator = GetComponent<Animator>();
            hpBar = GetComponentInChildren<EnemyHPBar>();
            spriteRenderer = GetComponent<SpriteRenderer>();

            if (spriteRenderer != null)
            {
                originalColor = spriteRenderer.color;
            }

            // Init HP bar
            if (hpBar != null)
            {
                hpBar.Init(maxHP);
            }
        }

        private void Start()
        {
            // If waypoints not set manually, find them from WaypointManager
            if (waypoints == null || waypoints.Length == 0)
            {
                WaypointManager waypointManager = FindObjectOfType<WaypointManager>();
                if (waypointManager != null)
                {
                    waypoints = waypointManager.GetWaypoints();
                }
            }

            // Validate waypoints
            if (waypoints == null || waypoints.Length == 0)
            {
                Debug.LogError("Enemy has no waypoints assigned!");
            }
        }

        private void Update()
        {
            if (isDead || hasReachedEnd) return;

            FindCombatTarget();

            // Nếu đang có mục tiêu hợp lệ → chiến đấu
            if (HasValidTarget())
            {
                Vector3 targetPos = GetTargetPosition();
                float distance = Vector2.Distance(transform.position, targetPos);

                // Flip sprite về phía target
                FlipTowards(targetPos);

                if (distance <= attackRange)
                {
                    // Trong tầm tấn công → đứng lại và tấn công
                    SetMovingAnimation(false);
                    TryAttackTarget();
                }
                else
                {
                    // Tiến đến mục tiêu
                    SetMovingAnimation(true);
                    Vector2 newPos = Vector2.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
                    transform.position = newPos;
                }
            }
            else
            {
                // Không có target → tiếp tục đi theo waypoint
                MoveAlongPath();
            }
        }

        // ======================
        // COMBAT AI - TARGET
        // ======================
        private void FindCombatTarget()
        {
            // --- Validate target unit hiện tại ---
            if (currentUnitTarget != null)
            {
                bool unitDead = !currentUnitTarget.IsAlive;
                bool unitTooFarFromPath = GetDistanceToNearestWaypointSegment(currentUnitTarget.transform.position) > maxChaseOffPathDistance;

                if (unitDead || unitTooFarFromPath)
                    currentUnitTarget = null;
            }

            // --- Validate target player hiện tại ---
            if (currentPlayerTarget != null)
            {
                bool playerDead = !currentPlayerTarget.IsAlive;
                bool playerTooFarFromPath = GetDistanceToNearestWaypointSegment(currentPlayerTarget.transform.position) > maxChaseOffPathDistance;

                if (playerDead || playerTooFarFromPath)
                    currentPlayerTarget = null;
            }

            // Nếu đã có target hợp lệ, không cần tìm mới
            if (HasValidTarget()) return;

            // --- Tìm player trong tầm phát hiện, chỉ nếu gần đường ---
            PlayerController[] players = FindObjectsOfType<PlayerController>();
            float closestPlayerDist = float.MaxValue;
            PlayerController closestPlayer = null;

            foreach (var player in players)
            {
                if (!player.IsAlive) continue;
                float dist = Vector2.Distance(transform.position, player.transform.position);
                if (dist > detectionRange || dist >= closestPlayerDist) continue;

                // Chỉ chấp nhận nếu player đang gần đường waypoint
                if (GetDistanceToNearestWaypointSegment(player.transform.position) <= maxChaseOffPathDistance)
                {
                    closestPlayerDist = dist;
                    closestPlayer = player;
                }
            }

            // --- Tìm unit đồng minh trong tầm phát hiện, chỉ nếu gần đường ---
            UnitCombat[] units = FindObjectsOfType<UnitCombat>();
            float closestUnitDist = float.MaxValue;
            UnitCombat closestUnit = null;

            foreach (var unit in units)
            {
                if (!unit.IsAlive) continue;
                float dist = Vector2.Distance(transform.position, unit.transform.position);
                if (dist > detectionRange || dist >= closestUnitDist) continue;

                // Chỉ chấp nhận nếu unit đang gần đường waypoint
                if (GetDistanceToNearestWaypointSegment(unit.transform.position) <= maxChaseOffPathDistance)
                {
                    closestUnitDist = dist;
                    closestUnit = unit;
                }
            }

            // --- Quyết định target ---
            if (prioritizePlayer && closestPlayer != null)
            {
                currentPlayerTarget = closestPlayer;
                currentUnitTarget = null;
            }
            else if (closestUnit != null && closestPlayer != null)
            {
                if (closestUnitDist <= closestPlayerDist)
                {
                    currentUnitTarget = closestUnit;
                    currentPlayerTarget = null;
                }
                else
                {
                    currentPlayerTarget = closestPlayer;
                    currentUnitTarget = null;
                }
            }
            else if (closestUnit != null)
            {
                currentUnitTarget = closestUnit;
                currentPlayerTarget = null;
            }
            else if (closestPlayer != null)
            {
                currentPlayerTarget = closestPlayer;
                currentUnitTarget = null;
            }
        }

        /// <summary>
        /// Tính khoảng cách ngắn nhất từ điểm <paramref name="point"/> đến
        /// bất kỳ đoạn thẳng nào trên đường waypoint.
        /// Dùng để kiểm tra target có đang "loanh quanh trên đường" hay không.
        /// </summary>
        private float GetDistanceToNearestWaypointSegment(Vector2 point)
        {
            if (waypoints == null || waypoints.Length == 0)
                return 0f; // Không có waypoint → không giới hạn

            float minDist = float.MaxValue;

            // Kiểm tra khoảng cách đến từng điểm waypoint
            for (int i = 0; i < waypoints.Length; i++)
            {
                if (waypoints[i] == null) continue;
                float d = Vector2.Distance(point, waypoints[i].position);
                if (d < minDist) minDist = d;
            }

            // Kiểm tra khoảng cách đến từng đoạn thẳng (segment) giữa 2 waypoint liền kề
            for (int i = 0; i < waypoints.Length - 1; i++)
            {
                if (waypoints[i] == null || waypoints[i + 1] == null) continue;

                Vector2 segA = waypoints[i].position;
                Vector2 segB = waypoints[i + 1].position;
                float d = DistancePointToSegment(point, segA, segB);
                if (d < minDist) minDist = d;
            }

            return minDist;
        }

        /// <summary>Khoảng cách ngắn nhất từ điểm P đến đoạn thẳng AB.</summary>
        private static float DistancePointToSegment(Vector2 p, Vector2 a, Vector2 b)
        {
            Vector2 ab = b - a;
            float lenSq = ab.sqrMagnitude;
            if (lenSq < 0.0001f) return Vector2.Distance(p, a); // a ≈ b

            float t = Mathf.Clamp01(Vector2.Dot(p - a, ab) / lenSq);
            Vector2 projection = a + t * ab;
            return Vector2.Distance(p, projection);
        }

        private bool HasValidTarget()
        {
            if (currentPlayerTarget != null && currentPlayerTarget.IsAlive) return true;
            if (currentUnitTarget != null && currentUnitTarget.IsAlive) return true;
            return false;
        }

        private Vector3 GetTargetPosition()
        {
            if (currentPlayerTarget != null && currentPlayerTarget.IsAlive)
                return currentPlayerTarget.transform.position;
            if (currentUnitTarget != null && currentUnitTarget.IsAlive)
                return currentUnitTarget.transform.position;
            return transform.position;
        }

        private void TryAttackTarget()
        {
            if (Time.time - lastAttackTime < attackCooldown) return;
            lastAttackTime = Time.time;

            // Play attack animation (DealDamageToTarget sẽ được gọi bởi animation event)
            if (animator != null)
            {
                animator.ResetTrigger("Attack"); // Tránh trigger bị tích lũy trong queue
                animator.SetTrigger("Attack");
            }
            else
            {
                // Nếu không có animator, gây sát thương ngay
                DealDamageToTarget();
            }
        }

        /// <summary>
        /// Gọi từ Animation Event tại frame tấn công, hoặc trực tiếp khi không có animator.
        /// </summary>
        public void DealDamageToTarget()
        {
            // Tấn công Player
            if (currentPlayerTarget != null && currentPlayerTarget.IsAlive)
            {
                float dist = Vector2.Distance(transform.position, currentPlayerTarget.transform.position);
                if (dist <= attackRange + 0.5f)
                {
                    currentPlayerTarget.TakeDamage(attackDamage);
                    Debug.Log($"[ENEMY] Attacked Player for {attackDamage} damage.");
                }
                return;
            }

            // Tấn công Unit
            if (currentUnitTarget != null && currentUnitTarget.IsAlive)
            {
                float dist = Vector2.Distance(transform.position, currentUnitTarget.transform.position);
                if (dist <= attackRange + 0.5f)
                {
                    currentUnitTarget.TakeDamage(attackDamage);
                    Debug.Log($"[ENEMY] Attacked Unit for {attackDamage} damage.");
                }
            }
        }

        // ======================
        // MOVEMENT
        // ======================
        private void MoveAlongPath()
        {
            if (waypoints == null || waypoints.Length == 0) return;
            if (currentWaypointIndex >= waypoints.Length)
            {
                ReachEnd();
                return;
            }

            Transform targetWaypoint = waypoints[currentWaypointIndex];
            if (targetWaypoint == null)
            {
                currentWaypointIndex++;
                return;
            }

            // Calculate distance before moving
            float distance = Vector2.Distance(transform.position, targetWaypoint.position);

            // Check if reached waypoint BEFORE moving
            if (distance <= waypointReachDistance)
            {
                currentWaypointIndex++;
                return; // Skip to next waypoint immediately
            }

            // Move towards waypoint
            Vector2 currentPos = transform.position;
            Vector2 targetPos = targetWaypoint.position;
            Vector2 newPos = Vector2.MoveTowards(currentPos, targetPos, moveSpeed * Time.deltaTime);
            transform.position = newPos;

            // Flip sprite based on movement direction
            FlipTowards(targetPos);

            SetMovingAnimation(true);
        }

        private void ReachEnd()
        {
            if (hasReachedEnd) return;

            hasReachedEnd = true;
            SetMovingAnimation(false);

            // Damage player's base
            GameManager gameManager = FindObjectOfType<GameManager>();
            if (gameManager != null)
            {
                gameManager.TakeDamage(damageToBase);
            }

            // Destroy enemy
            Destroy(gameObject);
        }

        // ======================
        // HELPERS
        // ======================
        private void FlipTowards(Vector3 targetPos)
        {
            if (spriteRenderer == null) return;
            float dirX = targetPos.x - transform.position.x;
            if (Mathf.Abs(dirX) > 0.01f)
            {
                spriteRenderer.flipX = dirX < 0;
            }
        }

        private void SetMovingAnimation(bool isMoving)
        {
            if (animator != null)
            {
                animator.SetBool("IsMoving", isMoving);
            }
        }

        // ======================
        // COMBAT - TAKE DAMAGE
        // ======================
        public void TakeDamage(int damage)
        {
            if (isDead) return;

            currentHP -= damage;
            currentHP = Mathf.Max(currentHP, 0);

            // Update HP bar
            if (hpBar != null)
            {
                hpBar.SetHP(currentHP);
            }

            // Flash effect
            StartCoroutine(DamageFlash());

            // Play hit animation if exists
            if (animator != null)
            {
                animator.SetTrigger("Hit");
            }

            if (currentHP <= 0)
            {
                Die();
            }
        }

        private IEnumerator DamageFlash()
        {
            if (spriteRenderer == null) yield break;

            spriteRenderer.color = damageFlashColor;
            yield return new WaitForSeconds(damageFlashDuration);
            spriteRenderer.color = originalColor;
        }

        private void Die()
        {
            if (isDead) return;
            isDead = true;

            Debug.Log("[ENEMY] Die() called!");

            // Give reward
            GameManager gameManager = FindObjectOfType<GameManager>();
            if (gameManager != null)
            {
                gameManager.AddGold(goldReward);
            }

            // Spawn death effect
            if (deathEffectPrefab != null)
            {
                Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
            }

            // Play death animation
            if (animator != null)
            {
                Debug.Log("[ENEMY] Playing death animation - SetTrigger('Die')");
                animator.SetTrigger("Die");
                // DestroyAfterDeath() will be called by animation event
            }
            else
            {
                Debug.LogWarning("[ENEMY] No animator found! Destroying immediately.");
                Destroy(gameObject);
            }
        }

        // ======================
        // ANIMATION EVENT
        // ======================
        public void DestroyAfterDeath()
        {
            Destroy(gameObject);
        }

        // ======================
        // GIZMOS (for debugging)
        // ======================
        private void OnDrawGizmos()
        {
            if (waypoints == null || waypoints.Length == 0) return;

            Gizmos.color = Color.yellow;
            for (int i = 0; i < waypoints.Length - 1; i++)
            {
                if (waypoints[i] != null && waypoints[i + 1] != null)
                {
                    Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            // Detection range
            Gizmos.color = new Color(1f, 0.5f, 0f, 0.4f); // Orange
            Gizmos.DrawWireSphere(transform.position, detectionRange);

            // Attack range
            Gizmos.color = new Color(1f, 0f, 0f, 0.6f); // Red
            Gizmos.DrawWireSphere(transform.position, attackRange);

            // Line to current target
            Vector3 targetPos = GetTargetPosition();
            if (HasValidTarget() && Application.isPlaying)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position, targetPos);
            }
        }
    }
}
