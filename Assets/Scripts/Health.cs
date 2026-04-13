using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    public float maxHealth = 100f;
    public float health;
    public UnityEvent<float, float> OnHealthChanged; // current, max
    private void Awake()
    {
        health = maxHealth;
    }
    public void TakeDamage(float amount)
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
