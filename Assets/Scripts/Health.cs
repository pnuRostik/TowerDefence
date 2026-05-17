using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    public float maxHealth = 100f;
    public float health;
    public UnityEvent<float, float> OnHealthChanged; 
    private void Awake()
    {
        health = maxHealth;
    }
    public void TakeDamage(float amount)
    {
        if (health <= 0) return; 

        health -= amount;
        if (health < 0) health = 0;

        OnHealthChanged?.Invoke(health, maxHealth);
    }

    public void ResetHealth()
    {
        health = maxHealth;
        OnHealthChanged?.Invoke(health, maxHealth);
    }


}
