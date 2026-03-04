using UnityEngine;
using UnityEngine.InputSystem; // New Input System
using Enemy;

/// <summary>
/// Test script to damage enemy by pressing Space key
/// Attach this to any GameObject in the scene (e.g., Main Camera)
/// Uses NEW Input System (UnityEngine.InputSystem)
/// </summary>
public class TestDamageEnemy : MonoBehaviour
{
    [Header("Test Settings")]
    [SerializeField] private int damageAmount = 10;

    void Update()
    {
        // Check if Keyboard is available
        if (Keyboard.current == null) return;

        // Check if Space key is pressed (New Input System)
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            // Find first enemy in scene
            EnemyController enemy = FindObjectOfType<EnemyController>();
            
            if (enemy != null)
            {
                Debug.Log($"[TEST] Dealing {damageAmount} damage to enemy!");
                enemy.TakeDamage(damageAmount);
            }
            else
            {
                Debug.LogWarning("[TEST] No enemy found in scene!");
            }
        }
    }
}
