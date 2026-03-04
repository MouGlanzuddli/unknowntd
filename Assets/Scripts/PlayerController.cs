using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private Animator animator;

    [Header("Combat")]
    [SerializeField] private float attackCooldown = 0.8f;

    [Header("Stats")]
    [SerializeField] private int maxHP = 100;

    [Header("Visual Effects")]
    [SerializeField] private Color damageFlashColor = Color.red;
    [SerializeField] private float damageFlashDuration = 0.15f;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private float attackTimer;
    private int currentHP;
    private bool isDead = false;
    private Color originalColor;

    private Vector2 playerMoveDirection;

    // REQUIRED BY ENEMY COMBAT RULES
    public bool IsAlive => !isDead;

    private void Awake()
    {
        currentHP = maxHP;

        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;
    }

    void Update()
    {
        if (isDead) return;

        // move
        if (Keyboard.current == null) return;

        float inputx =
            (Keyboard.current.dKey.isPressed ? 1f : 0f) -
            (Keyboard.current.aKey.isPressed ? 1f : 0f);

        float inputy =
            (Keyboard.current.wKey.isPressed ? 1f : 0f) -
            (Keyboard.current.sKey.isPressed ? 1f : 0f);

        playerMoveDirection = new Vector2(inputx, inputy).normalized;

        if (animator != null)
        {
            if (playerMoveDirection != Vector2.zero)
            {
                animator.SetFloat("LastMoveX", playerMoveDirection.x);
                animator.SetFloat("LastMoveY", playerMoveDirection.y);
            }
            animator.SetFloat("MoveX", playerMoveDirection.x);
            animator.SetFloat("MoveY", playerMoveDirection.y);
            animator.SetBool("IsMoving", playerMoveDirection != Vector2.zero);
        }

        // fight
        attackTimer -= Time.deltaTime;

        if (attackTimer <= 0f)
        {
            if (animator != null)
                animator.SetBool("IsAttacking", true);
            attackTimer = attackCooldown;
        }
    }

    public void EndAttack()
    {
        if (animator != null)
            animator.SetBool("IsAttacking", false);
    }

    void FixedUpdate()
    {
        if (isDead)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }
        rb.linearVelocity = playerMoveDirection * moveSpeed;
    }

    // ======================
    // COMBAT - TAKE DAMAGE
    // ======================
    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHP -= damage;
        currentHP = Mathf.Max(currentHP, 0);

        Debug.Log($"[PLAYER] Took {damage} damage! HP: {currentHP}/{maxHP}");

        // Flash effect
        if (spriteRenderer != null)
            StartCoroutine(DamageFlash());

        // Play hit animation
        if (animator != null)
            animator.SetTrigger("Hit");

        if (currentHP <= 0)
        {
            Die();
        }
    }

    private IEnumerator DamageFlash()
    {
        spriteRenderer.color = damageFlashColor;
        yield return new WaitForSeconds(damageFlashDuration);
        spriteRenderer.color = originalColor;
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log("[PLAYER] Player has died!");

        if (animator != null)
            animator.SetTrigger("Die");

        // Thông báo game over
        GameManager gameManager = FindObjectOfType<GameManager>();
        if (gameManager != null)
        {
            // Bạn có thể gọi gameManager.GameOver() nếu có method này
            // gameManager.GameOver();
        }
    }
}
