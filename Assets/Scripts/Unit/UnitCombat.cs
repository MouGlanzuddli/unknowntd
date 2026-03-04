using UnityEngine;
using System.Collections;
using Enemy;

namespace Unit
{
    public class UnitCombat : MonoBehaviour
    {
        [Header("Stats")]
        [SerializeField] private int maxHP = 50;
        [SerializeField] private int damage = 10;
        [SerializeField] private int cost = 50;

        [Header("Combat")]
        [SerializeField] private float attackRange = 1.5f;
        [SerializeField] private float detectionRange = 5f;
        [SerializeField] private float attackCooldown = 1f;

        [Header("Movement")]
        [SerializeField] private float moveSpeed = 2f;
        [SerializeField] private float stopDistance = 0.8f;
        [SerializeField] private float returnSpeed = 1.5f; // Speed when returning to spawn
        [SerializeField] private float spawnReachDistance = 0.2f; // Distance to consider "reached spawn"

        [Header("Visual Effects")]
        [SerializeField] private Color damageFlashColor = Color.red;
        [SerializeField] private float damageFlashDuration = 0.1f;

        private int currentHP;
        private float lastAttackTime;
        private EnemyController currentTarget;
        private bool isDead = false;
        private Vector2 spawnPosition; // Store spawn position

        private Animator animator;
        private SpriteRenderer spriteRenderer;
        private Color originalColor;
        private UnitSpawner spawner;

        public bool IsAlive => !isDead;
        public int Cost => cost;

        private void Awake()
        {
            currentHP = maxHP;
            animator = GetComponent<Animator>();
            spriteRenderer = GetComponent<SpriteRenderer>();

            // Save spawn position
            spawnPosition = transform.position;

            if (spriteRenderer != null)
            {
                originalColor = spriteRenderer.color;
            }
        }

        private void Start()
        {
            // Find spawner to notify on death
            spawner = FindObjectOfType<UnitSpawner>();
        }

        private void Update()
        {
            if (isDead) return;

            FindTarget();

            // No target found - return to spawn position
            if (currentTarget == null || !currentTarget.IsAlive)
            {
                ReturnToSpawn();
                return;
            }

            // Has target - engage in combat
            float distance = Vector2.Distance(transform.position, currentTarget.transform.position);

            if (distance > attackRange + stopDistance)
            {
                MoveToTarget();
            }
            else
            {
                TryAttack();
            }
        }

        private void FindTarget()
        {
            // Keep current target if still valid and in range
            if (currentTarget != null && currentTarget.IsAlive)
            {
                float dist = Vector2.Distance(transform.position, currentTarget.transform.position);
                if (dist <= detectionRange)
                    return;
            }

            // Find new target within detection range
            EnemyController[] enemies = FindObjectsOfType<EnemyController>();
            float closestDist = detectionRange;
            EnemyController closest = null;

            foreach (var enemy in enemies)
            {
                if (!enemy.IsAlive) continue;

                float dist = Vector2.Distance(transform.position, enemy.transform.position);
                if (dist < closestDist)
                {
                    closestDist = dist;
                    closest = enemy;
                }
            }

            currentTarget = closest;
        }

        private void ReturnToSpawn()
        {
            float distanceToSpawn = Vector2.Distance(transform.position, spawnPosition);

            // Already at spawn position
            if (distanceToSpawn <= spawnReachDistance)
            {
                // Stop moving - idle animation
                if (animator != null)
                {
                    animator.SetBool("IsMoving", false);
                }
                return;
            }

            // Move back to spawn position
            Vector2 currentPos = transform.position;
            Vector2 direction = (spawnPosition - currentPos).normalized;
            Vector2 newPos = Vector2.MoveTowards(currentPos, spawnPosition, returnSpeed * Time.deltaTime);
            transform.position = newPos;

            // Flip sprite based on direction
            if (spriteRenderer != null && Mathf.Abs(direction.x) > 0.01f)
            {
                spriteRenderer.flipX = direction.x < 0;
            }

            // Play movement animation
            if (animator != null)
            {
                animator.SetBool("IsMoving", true);
            }
        }

        private void MoveToTarget()
        {
            if (currentTarget == null) return;

            Vector2 currentPos = transform.position;
            Vector2 targetPos = currentTarget.transform.position;
            Vector2 direction = (targetPos - currentPos).normalized;

            // Move towards target
            Vector2 newPos = Vector2.MoveTowards(currentPos, targetPos, moveSpeed * Time.deltaTime);
            transform.position = newPos;

            // Flip sprite based on direction
            if (spriteRenderer != null && Mathf.Abs(direction.x) > 0.01f)
            {
                spriteRenderer.flipX = direction.x < 0;
            }

            // Play movement animation
            if (animator != null)
            {
                animator.SetBool("IsMoving", true);
            }
        }

        private void TryAttack()
        {
            // Stop moving
            if (animator != null)
            {
                animator.SetBool("IsMoving", false);
            }

            // Check cooldown
            if (Time.time - lastAttackTime < attackCooldown)
                return;

            // Face target
            if (currentTarget != null && spriteRenderer != null)
            {
                float dirX = currentTarget.transform.position.x - transform.position.x;
                if (Mathf.Abs(dirX) > 0.01f)
                {
                    spriteRenderer.flipX = dirX < 0;
                }
            }

            lastAttackTime = Time.time;

            // Play attack animation
            if (animator != null)
            {
                animator.SetTrigger("Attack");
            }
            else
            {
                // If no animator, deal damage immediately
                DealDamage();
            }
        }

        // ======================
        // COMBAT - DAMAGE
        // ======================
        public void TakeDamage(int damageAmount)
        {
            if (isDead) return;

            currentHP -= damageAmount;
            currentHP = Mathf.Max(currentHP, 0);

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

            // Notify spawner
            if (spawner != null)
            {
                spawner.NotifyUnitDestroyed();
            }

            // Play death animation
            if (animator != null)
            {
                animator.SetTrigger("Die");
                // DestroyAfterDeath() will be called by animation event
            }
            else
            {
                Destroy(gameObject);
            }
        }

        // ======================
        // ANIMATION EVENTS
        // ======================
        
        // Called at hit frame of attack animation
        public void DealDamage()
        {
            if (currentTarget != null && currentTarget.IsAlive)
            {
                float distance = Vector2.Distance(transform.position, currentTarget.transform.position);
                if (distance <= attackRange + 0.5f) // Small tolerance
                {
                    currentTarget.TakeDamage(damage);
                }
            }
        }

        // Called at end of death animation
        public void DestroyAfterDeath()
        {
            Destroy(gameObject);
        }

        // ======================
        // GIZMOS (for debugging)
        // ======================
        private void OnDrawGizmosSelected()
        {
            // Attack range
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);

            // Detection range
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, detectionRange);

            // Spawn position (show in both edit and play mode)
            Vector2 spawnPos = Application.isPlaying ? spawnPosition : (Vector2)transform.position;
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(spawnPos, 0.3f);
            Gizmos.DrawLine(spawnPos, spawnPos + Vector2.up * 0.5f);

            // Line to spawn position (when returning)
            if (Application.isPlaying && (currentTarget == null || !currentTarget.IsAlive))
            {
                float dist = Vector2.Distance(transform.position, spawnPosition);
                if (dist > spawnReachDistance)
                {
                    Gizmos.color = Color.blue;
                    Gizmos.DrawLine(transform.position, spawnPosition);
                }
            }

            // Line to current target
            if (currentTarget != null && Application.isPlaying)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(transform.position, currentTarget.transform.position);
            }
        }
    }
}
