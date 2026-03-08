using System;
using UnityEngine;
using UnityEngine.UI;

public class Health
{
    private float maxHealth;
    private float currentHealth;
    private Image healthBar;
    public bool IsDead => currentHealth <= 0;

    public event Action OnDeath;

    public Health(float maxHealth, Image healthBar = null)
    {
        this.maxHealth = maxHealth;
        this.healthBar = healthBar;

        currentHealth = maxHealth;
        UpdateUI();
    }
    public void SetHealthBar(Image healthBar)
    {
        this.healthBar = healthBar;
        UpdateUI();
    }
    public void Damage(float amount)
    {
        if (IsDead) return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        UpdateUI();

        if (currentHealth <= 0)
        {
            OnDeath?.Invoke();
        }
    }

    public void Heal(float amount)
    {
        if (IsDead) return;

        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        UpdateUI();
    }

    private void UpdateUI()
    {
        if (healthBar != null)
            healthBar.fillAmount = currentHealth / maxHealth;
    }
}