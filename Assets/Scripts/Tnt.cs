using System.Collections.Generic;
using UnityEngine;

public class Tnt : MonoBehaviour
{
    private float damage;
    private Vector3 startPos;
    private Vector3 targetPos;
    private float duration;
    private float elapsed;
    private bool isFlying;

    [Header("Settings")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float arcHeight = 1.5f;
    [SerializeField] private float explosionRadius = 1.5f;
    [SerializeField] private GameObject explosionEffectPrefab;

    public void Init(float damage, Vector3 target, float radius)
    {
        this.damage = damage;
        this.targetPos = target;
        this.startPos = transform.position;
        this.explosionRadius = radius;
        this.elapsed = 0f;
        this.isFlying = true;

        // // Cập nhật bán kính collider nếu cần thiết (để debug hoặc trigger)
        // CircleCollider2D col = GetComponent<CircleCollider2D>();
        // if (col != null)
        // {
        //     col.radius = explosionRadius;
        // }

        float distance = Vector3.Distance(startPos, targetPos);
        duration = Mathf.Max(0.2f, distance / speed);
    }

    private void Update()
    {
        if (!isFlying) return;

        elapsed += Time.deltaTime;
        float t = elapsed / duration;

        if (t >= 1f)
        {
            transform.position = targetPos;
            isFlying = false;
            OnHitTarget();
            return;
        }

        Vector3 currentGroundPos = Vector3.Lerp(startPos, targetPos, t);
        float arcOffset = 4 * arcHeight * t * (1 - t);
        transform.position = new Vector3(currentGroundPos.x, currentGroundPos.y + arcOffset, currentGroundPos.z);
        transform.Rotate(0, 0, 720 * Time.deltaTime);
    }

    private void OnHitTarget()
    {
        // Xử lý nổ: tìm tất cả các mục tiêu trong phạm vi
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        
        // Dùng HashSet để tránh gây sát thương nhiều lần cho một đối tượng có nhiều collider
        HashSet<IDamageable> damageables = new HashSet<IDamageable>();

        foreach (var col in colliders)
        {
            IDamageable damageable = col.GetComponentInParent<IDamageable>();
            if (damageable != null)
            {
                // Kiểm tra nếu là BaseUnitController thì lọc theo faction
                if (damageable is BaseUnitController unit)
                {
                    // TNT của Boss không gây dame cho kẻ địch (đồng đội của boss)
                    if (unit.Data.faction == Faction.Enemy) continue;
                }

                damageables.Add(damageable);
            }
        }

        foreach (var target in damageables)
        {
            target.TakeDamage(damage);
        }

        // Spawn hiệu ứng nổ
        if (explosionEffectPrefab != null)
        {
            GameObject effect = Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
            
            // Tự động xoá hiệu ứng sau khi nổ xong (thường 2s là đủ)
            ParticleSystem ps = effect.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                Destroy(effect, ps.main.duration + ps.main.startLifetime.constantMax);
            }
            else
            {
                Destroy(effect, 2f);
            }
        }

        Destroy(gameObject);
    }

    // Vẽ phạ mvi nổ trong Editor để dễ quan sát
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
