using UnityEngine;

public class BaseHealth : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }
    // condition: in Enemy.cs
    //public int damage = 10;

    //void ReachEnd()
    //{
    //    FindObjectOfType<BaseHealth>().TakeDamage(damage);
    //    WaveSpawner.enemiesAlive--;
    //    Destroy(gameObject);
    //}
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            GameOver();
        }
    }

    void GameOver()
    {
        GameStateManager.Instance.SetState(GameState.GameOver);
    }
}