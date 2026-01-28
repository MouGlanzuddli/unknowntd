using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private Animator animator;
    [SerializeField] private float attackCooldown = 0.8f;
    private float attackTimer;



    private Vector2 playerMoveDirection;

    void Update()
    {
        // move
        if (Keyboard.current == null) return;

        float inputx =
            (Keyboard.current.dKey.isPressed ? 1f : 0f) -
            (Keyboard.current.aKey.isPressed ? 1f : 0f);

        float inputy =
            (Keyboard.current.wKey.isPressed ? 1f : 0f) -
            (Keyboard.current.sKey.isPressed ? 1f : 0f);

        playerMoveDirection = new Vector2(inputx, inputy).normalized;
        if (playerMoveDirection != Vector2.zero)
        {
            animator.SetFloat("LastMoveX", playerMoveDirection.x);
            animator.SetFloat("LastMoveY", playerMoveDirection.y);
        }
        animator.SetFloat("MoveX", playerMoveDirection.x);
        animator.SetFloat("MoveY", playerMoveDirection.y);
        animator.SetBool("IsMoving", playerMoveDirection != Vector2.zero);

        // fight
        attackTimer -= Time.deltaTime;

        if (attackTimer <= 0f)
        {
            animator.SetBool("IsAttacking", true);
            attackTimer = attackCooldown;
        }
    }

    public void EndAttack()
    {
        animator.SetBool("IsAttacking", false);
    }

    void FixedUpdate()
    {
        rb.linearVelocity = playerMoveDirection * moveSpeed;
    }


}
