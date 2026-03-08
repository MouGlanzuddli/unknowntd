using Pathfinding;
using UnityEngine;

public class UnitMovement : BaseMovement
{
    private AIPath aiPath;
    private Transform currentTarget;
    private float stopDistance;
    private bool isStoppedByRange;

    public UnitMovement(BaseUnitController owner, AIPath aiPath, float speed, Animator animator = null) 
        : base(owner, speed, animator)
    {
        this.aiPath = aiPath;

        if (aiPath != null)
        {
            aiPath.maxSpeed = speed;
        }

        AstarPath.active.logPathResults = PathLog.None;
    }

    public override void SetSpeed(float speed)
    {
        base.SetSpeed(speed);
        if (aiPath != null) aiPath.maxSpeed = speed;
    }

    public override void SetTarget(Transform target, float stopDistance)
    {
        if (target == null) return;

        currentTarget = target;
        this.stopDistance = stopDistance;

        if (aiPath != null)
        {
            aiPath.endReachedDistance = stopDistance;
            aiPath.isStopped = false;
            aiPath.destination = target.position;
        }

        isStoppedByRange = false;
    }

    public override void ClearTarget()
    {
        currentTarget = null;
        if (aiPath != null) aiPath.isStopped = true;
        UpdateAnimation(Vector2.zero, false);
    }

    public override void Tick()
    {
        if (currentTarget == null || aiPath == null)
            return;

        float sqrDist = (currentTarget.position - owner.transform.position).sqrMagnitude;
        float stopSqr = stopDistance * stopDistance;

        if (sqrDist > stopSqr)
        {
            if (isStoppedByRange)
            {
                aiPath.isStopped = false;
                isStoppedByRange = false;
            }

            aiPath.destination = currentTarget.position;
        }
        else
        {
            if (!isStoppedByRange)
            {
                aiPath.isStopped = true;
                isStoppedByRange = true;
                UpdateAnimation(Vector2.zero, false);
            }
        }

        if (!isStoppedByRange)
        {
            bool walking = aiPath.velocity.sqrMagnitude > 0.0025f; 
            UpdateAnimation(aiPath.velocity.normalized, walking);
        }
    }
}