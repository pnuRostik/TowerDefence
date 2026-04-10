using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    public int maxHealth = 100;
    public int health;
    public UnityEvent<int, int> OnHealthChanged; // current, max
    private void Awake()
    {
        health = maxHealth;
    }
    public void TakeDamage(int amount)
    {
        health -= amount;
        if (health < 0)
            health = 0;

        OnHealthChanged?.Invoke(health, maxHealth);
        if (health == 0)
            Die();
    }

    void Die()
    {
        gameObject.SetActive(false);
    }
}
