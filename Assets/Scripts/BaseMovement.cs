using UnityEngine;

public abstract class BaseMovement
{
    protected BaseUnitController owner;
    protected Animator animator;
    protected float moveSpeed;

    protected readonly int moveXHash = Animator.StringToHash("MoveX");
    protected readonly int moveYHash = Animator.StringToHash("MoveY");
    protected readonly int isWalkingHash = Animator.StringToHash("IsWalking");

    public BaseMovement(BaseUnitController owner, float moveSpeed, Animator animator)
    {
        this.owner = owner;
        this.moveSpeed = moveSpeed;
        this.animator = animator;
    }

    public virtual void SetSpeed(float speed) 
    { 
        moveSpeed = speed; 
    }
    
    public abstract void Tick();
    public virtual void FixedTick() { }

    public virtual void SetTarget(Transform target, float stopDistance) { }
    public virtual void SetTarget(BaseUnitController unit, float stopDistance) 
    {
        if (unit != null) SetTarget(unit.transform, stopDistance);
    }
    public virtual void ClearTarget() { }

    protected virtual void UpdateAnimation(Vector2 direction, bool isWalking)
    {
        if (animator == null) return;

        animator.SetBool(isWalkingHash, isWalking);

        if (isWalking)
        {
            animator.SetFloat(moveXHash, direction.x);
            animator.SetFloat(moveYHash, direction.y);
        }
    }
}
