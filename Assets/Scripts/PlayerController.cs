using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : BaseUnitController
{
    [SerializeField] private CombatUnitData playerData;
    private Rigidbody2D rb;

    protected override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody2D>();

        if (aiPath != null)
        {
            aiPath.enabled = false;
        }

        if (playerData != null )
        {
            Init(playerData);
        }
    }

    protected override void InitLevel()
    {
        base.InitLevel();
        if (level != null)
        {
            level.OnLevelUp += HandleLevelUp;
        }
    }

    private void Start()
    {
        if (playerData != null)
        {
            PlayerUnitManager.Instance.RegisterPlayer(this);
        }

        BaseUnitController.OnAnyEnemyDeath += OnEnemyKilled;
    }

    private void OnDestroy()
    {
        BaseUnitController.OnAnyEnemyDeath -= OnEnemyKilled;
    }

    private void OnEnemyKilled(BaseUnitController victim)
    {
        if (level != null && victim != null && victim.Data.faction == Faction.Enemy)
        {
            level.AddExp(50);
        }
    }

    private void HandleLevelUp(int newLevel)
    {
        Debug.Log($"Player Level Up: {newLevel}");

        var spriteLibrary = GetComponentInChildren<UnityEngine.U2D.Animation.SpriteLibrary>();
        if (spriteLibrary != null)
        {
            var levelInfo = level.GetCurrentLevelInfo();
            if (levelInfo != null && levelInfo.spriteLibrary != null)
            {
                spriteLibrary.spriteLibraryAsset = levelInfo.spriteLibrary;
            }
        }
    }

    protected override void InitMovement()
    {
        movement = new PlayerMovement(this, rb, data.moveSpeed);
    }


    protected override void Update()
    {
        if (isAttacking) return;

        movement?.Tick();

        if (Input.GetMouseButtonDown(0))
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                TryAttack();
            }
        }
    }

    protected override void FixedUpdate()
    {
        if (isAttacking)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        movement?.FixedTick();
    }

    private void TryAttack()
    {
        if (isAttacking) return;
        if (Time.time < nextAttackTime) return;

        isAttacking = true;
        rb.linearVelocity = Vector2.zero;
        animator.SetTrigger(AttackTriggerHash);
        StartCoroutine(PlaySlashDelay());
    }


    private IEnumerator PlaySlashDelay()
    {
        yield return new WaitForSeconds(0.35f);

        if (AudioManager.Instance != null && isAttacking)
            AudioManager.Instance.PlaySwordSlash();
    }

    public override void AttackHit()
    {

        if (hasAppliedDamage) return;
        hasAppliedDamage = true;

        Vector2 lookDir = new Vector2(animator.GetFloat("MoveX"), animator.GetFloat("MoveY"));
        
        if (lookDir.sqrMagnitude < 0.01f) lookDir = Vector2.right; 
        lookDir.Normalize();

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, attacktRange);

        foreach (var hit in hitEnemies)
        {
            BaseUnitController unit = hit.GetComponent<BaseUnitController>();
            if (unit == null || unit.Data.faction != Faction.Enemy) continue;

            Vector2 dirToEnemy = (unit.transform.position - transform.position).normalized;

            float dot = Vector2.Dot(lookDir, dirToEnemy);

            if (dot >= 0)
            {
                unit.TakeDamage(data.damage);
            }
        }
    }

    public Animator GetAnimator()
    {
        return animator;
    }

    protected override void Death()
    {
        base.Death();
        GameManager.Instance.LoseGame();
    }
}