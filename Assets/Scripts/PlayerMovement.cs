using UnityEngine;

public class PlayerMovement : BaseMovement
{
    private Rigidbody2D rb;
    private Vector2 input;
    private Vector2 moveDirection;
    private bool isWalking;

    public PlayerMovement(BaseUnitController owner, Rigidbody2D rb, float moveSpeed) 
        : base(owner, moveSpeed, owner.GetComponentInChildren<Animator>())
    {
        this.rb = rb;
    }

    public override void Tick()
    {
        input.x = Input.GetAxisRaw("Horizontal");
        input.y = Input.GetAxisRaw("Vertical");

        moveDirection = input.normalized;

        isWalking = moveDirection != Vector2.zero;

        UpdateAnimation(moveDirection, isWalking);
    }

    public override void FixedTick()
    {
        if (rb != null)
        {
            rb.linearVelocity = moveDirection * moveSpeed;
        }
    }
}