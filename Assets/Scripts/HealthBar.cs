using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Health health;
    public Image fillimage;
    private void Start()
    {
        if (health != null)
        {
            health.OnHealthChanged.AddListener(UpdateBar);
            UpdateBar(health.health, health.maxHealth);
        }
    }
    void UpdateBar(int current, int max)
    {
        if (fillimage != null && max > 0)
        {
            fillimage.fillAmount = (float)current / max;
        }
    }
    private void OnDisable()
    {
        if (health != null)
        {
            health.OnHealthChanged.RemoveListener(UpdateBar);
        }
    }
}
