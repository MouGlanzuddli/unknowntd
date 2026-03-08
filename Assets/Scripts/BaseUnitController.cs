using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;


[RequireComponent(typeof(AIPath))]
public abstract class BaseUnitController : MonoBehaviour, IDamageable
{
    [Header("Components")]
    protected AIPath aiPath;                     // A* Pathfinding component dùng để di chuyển
    protected Animator animator;                 // Animator điều khiển animation
    protected SpriteRenderer spriteRenderer;     // Renderer hiển thị sprite và flip hướng
    protected UnitAnimationEvents animEvents;    // Script nhận Animation Event (attack, hit, ...)
    [SerializeField] protected Image healthBar;  // UI thanh máu của unit
    [SerializeField] protected Image expBar;     // UI thanh kinh nghiệm
    [SerializeField] protected TextMeshProUGUI levelTextLabel; // Text hiển thị level của unit

    [Header("Unit Data")]
    protected UnitData data;                     // ScriptableObject chứa dữ liệu unit
    protected float moveSpeed = 3f;              // Tốc độ di chuyển
    protected float attacktRange = 0.5f;         // Khoảng cách để có thể tấn công
    protected float aggroRange = 3f;             // Khoảng cách phát hiện mục tiêu
    public UnitData Data => data;                // Property expose dữ liệu unit

    [Header("Targeting")]
    protected Transform targetTransform;         // Transform của mục tiêu hiện tại
    protected BaseUnitController targetUnit;     // Reference tới controller của mục tiêu

    #region Systems
    protected Health health;                     // Hệ thống quản lý máu
    protected UnitLevel level;                   // Hệ thống level và exp
    protected BaseMovement movement;             // Hệ thống di chuyển của unit
    #endregion

    #region Global Events
    public static event System.Action<BaseUnitController> OnAnyEnemyDeath; // Event global khi bất kỳ enemy nào chết
    #endregion

    #region Combat State
    protected bool isAttacking;                  // Unit đang trong trạng thái tấn công
    protected bool hasAppliedDamage;             // Đã áp dụng damage trong animation hiện tại chưa
    protected float nextAttackTime;              // Thời điểm có thể tấn công tiếp theo
    protected float attackCooldown = 0.8f;       // Thời gian chờ giữa các lần tấn công
    #endregion

    #region Movement State
    protected bool isFacingRight;                // Hướng nhìn hiện tại của sprite
    #endregion

    #region Animator Hashes
    protected virtual int IsMovingHash => Animator.StringToHash("isRunning"); // Hash parameter chạy
    protected virtual int AttackTriggerHash => Animator.StringToHash("attack"); // Hash trigger attack
    private static readonly int HitTriggerHash = Animator.StringToHash("hit"); // Hash trigger bị đánh
    #endregion

    #region Events
    public event System.Action<BaseUnitController> OnDeath; // Event khi unit chết
    #endregion

    protected virtual void Awake()
    {
        aiPath = GetComponent<AIPath>();
        animator = GetComponentInChildren<Animator>();
        animEvents = GetComponentInChildren<UnitAnimationEvents>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    public virtual void Init(UnitData unitData)
    {
        data = unitData;

        moveSpeed = data.moveSpeed;
        attacktRange = data.attackRange;
        aggroRange = data.aggroRange;

        isFacingRight = data.isFacingRight;

        if (data is CombatUnitData combatData)
        {
            attackCooldown = combatData.attackCooldown;
        }

        if (aiPath != null)
        {
            aiPath.maxSpeed = moveSpeed;
        }

        InitMovement();
        InitHealth();
        InitLevel();
        BindAnimationEvents();
    }

    protected virtual void InitLevel()
    {
        if (data.levelData != null)
        {
            level = new UnitLevel(data.levelData, expBar, levelTextLabel);
        }
    }

    protected virtual void InitMovement()
    {
        movement = new UnitMovement(this, aiPath, moveSpeed, animator);
    }

    protected virtual void InitHealth()
    {
        health = new Health(data.maxHealth, healthBar);
        health.OnDeath += Death;
    }

    protected virtual void Update()
    {
        UpdateTarget();

        if (!IsTargetValid())
        {
            CancelAttack();
            ClearTarget();
        }

        if (!isAttacking)
        {
            movement?.Tick();
        }

        HandleFlip();
        HandleAnimation();
    }

    protected virtual void FixedUpdate()
    {
        if (!isAttacking)
        {
            movement?.FixedTick();
        }
    }

    protected virtual void UpdateTarget() { }

    protected virtual bool IsTargetValid()
    {
        return targetTransform != null || targetUnit != null;
    }

    protected virtual void CancelAttack()
    {
        isAttacking = false;
        if (aiPath != null) aiPath.isStopped = false;

        if (animator != null)
        {
            animator.ResetTrigger(AttackTriggerHash);
            animator.SetBool(IsMovingHash, false);
        }
    }

    protected virtual void HandleFlip()
    {
        if (aiPath == null) return;
        float dirX = aiPath.desiredVelocity.x;
        if (Mathf.Abs(dirX) > 0.01f) FlipTo(dirX);
    }

    protected virtual void HandleAnimation()
    {
        if (animator == null) return;

        if (isAttacking)
        {
            animator.SetBool(IsMovingHash, false);
            return;
        }

        bool moving = aiPath != null && aiPath.desiredVelocity.sqrMagnitude > 0.01f;
        animator.SetBool(IsMovingHash, moving);
    }

    public virtual void MoveTo(Transform target, float stopDistance = 0.1f)
    {
        movement?.SetTarget(target, stopDistance);
    }

    public virtual void MoveTo(BaseUnitController target, float stopDistance = 0.1f)
    {
        movement?.SetTarget(target, stopDistance);
    }

    public virtual void StopMove()
    {
        movement?.ClearTarget();
    }

    protected virtual void BindAnimationEvents()
    {
        if (animEvents == null) return;

        animEvents.OnHitStart += OnHitStart;
        animEvents.OnHit += OnHit;
        animEvents.OnHitEnd += OnHitEnd;

        animEvents.OnAttackStart += AttackStart;
        animEvents.OnAttackHit += AttackHit;
        animEvents.OnAttackEnd += AttackEnd;

        animEvents.OnMineStart += AttackStart;
        animEvents.OnMineHit += AttackHit;
        animEvents.OnMineEnd += AttackEnd;
    }

    #region Animation Event Handlers
    protected virtual void OnHitStart()
    {
        if (aiPath != null)
            aiPath.isStopped = true;
        if (AudioManager.Instance != null) AudioManager.Instance.PlayHit();

    }

    public virtual void OnHit()
    {
        isAttacking = false;
        if (aiPath != null)
            aiPath.isStopped = false;
        
    }

    protected virtual void OnHitEnd()
    {
        if (aiPath != null)
            aiPath.isStopped = false;
    }

    public virtual void AttackStart() 
    {
        hasAppliedDamage = false;
    }

    public virtual void AttackHit() { }

    public virtual void AttackEnd()
    {
        isAttacking = false;
        if (aiPath != null) aiPath.isStopped = false;
        nextAttackTime = Time.time + attackCooldown;
    }
    #endregion

    protected void FlipTo(float directionX)
    {
        if (directionX == 0) return;

        bool shouldFaceRight = directionX > 0;

        if (shouldFaceRight != isFacingRight)
        {
            Flip();
        }
    }

    protected void Flip()
    {
        isFacingRight = !isFacingRight;

        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = !spriteRenderer.flipX;
        }
    }

    protected void SetFacing(bool faceRight)
    {
        if (isFacingRight == faceRight) return;

        Flip();
    }

    public virtual void SetTarget(Transform target, float range)
    {
        targetTransform = target;
        targetUnit = null;

        movement?.SetTarget(target, range);
    }

    public virtual void SetTarget(Transform target)
    {
        SetTarget(target, attacktRange);
    }

    public virtual void SetTarget(BaseUnitController unit, float range)
    {
        targetUnit = unit;
        targetTransform = unit != null ? unit.transform : null;

        movement?.SetTarget(unit, range);
    }

    public virtual void SetTarget(BaseUnitController unit)
    {
        SetTarget(unit, attacktRange);
    }

    public virtual void ClearTarget()
    {
        targetTransform = null;
        targetUnit = null;

        StopMove();
    }

    protected Vector3 GetTargetPosition()
    {
        if (targetUnit != null)
            return targetUnit.transform.position;

        if (targetTransform != null)
            return targetTransform.position;

        return transform.position;
    }

    protected bool HasTarget()
    {
        return targetTransform != null;
    }

    protected BaseUnitController FindEnemyTarget()
    {
        return EnemyManager.Instance.GetClosestEnemy(transform.position);
    }

    public void TakeDamage(float amount)
    {
        if (health == null) return;

        health.Damage(amount);
        PlayHitAnimation();
    }

    protected virtual void PlayHitAnimation()
    {
        if (animator == null) return;

        if (isAttacking)
        {
            isAttacking = false;
            if (aiPath != null)
                aiPath.isStopped = false;
        }

        animator.SetTrigger(HitTriggerHash);
    }

    protected virtual void Death()
    {

        if (data.faction == Faction.Enemy)
        {
            EnemyManager.Instance.Unregister(this);
            OnAnyEnemyDeath?.Invoke(this);
        }

        OnDeath?.Invoke(this);
        Destroy(gameObject);
    }

    public void AddDeathListener(System.Action<BaseUnitController> callback)
    {
        OnDeath += callback;
    }

    public void RemoveDeathListener(System.Action<BaseUnitController> callback)
    {
        OnDeath -= callback;
    }

    protected Vector3 GetClosestTargetPoint()
    {
        Transform target = null;

        if (targetUnit != null)
            target = targetUnit.transform;
        else if (targetTransform != null)
            target = targetTransform;

        if (target == null)
            return transform.position;

        Collider2D col = target.GetComponent<Collider2D>();

        if (col != null)
            return col.ClosestPoint(transform.position);

        return target.position;
    }

    void OnDrawGizmosSelected()
    {
        // Attack range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attacktRange);

        // Aggro range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, aggroRange);
    }
}