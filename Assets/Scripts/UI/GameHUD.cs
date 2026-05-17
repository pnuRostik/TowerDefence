using UnityEngine;
using TMPro;

public class GameHUD : MonoBehaviour
{
    private TextMeshProUGUI hudText;
    private EnemySpawner spawner;
    private Health baseHealth;

    void Awake()
    {
        hudText = GetComponent<TextMeshProUGUI>();
        spawner = Object.FindAnyObjectByType<EnemySpawner>();
    }

    void Start()
    {
        // Setup Gold listener
        if (EconomyManager.Instance != null)
        {
            EconomyManager.Instance.OnGoldChanged += HandleGoldChanged;
        }

        // Setup Wave listener
        if (spawner != null)
        {
            spawner.OnWaveStarted += HandleWaveStarted;
        }

        // Setup Base Health listener
        GameObject tower = GameObject.FindWithTag("Tower");
        if (tower != null)
        {
            baseHealth = tower.GetComponent<Health>();
            if (baseHealth != null)
            {
                baseHealth.OnHealthChanged.AddListener(HandleHealthChanged);
            }
        }

        UpdateDisplay();
    }

    void OnDestroy()
    {
        if (EconomyManager.Instance != null)
        {
            EconomyManager.Instance.OnGoldChanged -= HandleGoldChanged;
        }
        if (spawner != null)
        {
            spawner.OnWaveStarted -= HandleWaveStarted;
        }
    }

    private void HandleGoldChanged(int newGold) => UpdateDisplay();
    private void HandleWaveStarted(int wave) => UpdateDisplay();
    private void HandleHealthChanged(float curr, float max) => UpdateDisplay();

    public void UpdateDisplay()
    {
        if (hudText == null) return;

        int gold = EconomyManager.Instance != null ? EconomyManager.Instance.gold : 0;
        int wave = GameManager.Instance != null ? GameManager.Instance.CurrentWave : 0;
        float hp = baseHealth != null ? baseHealth.health : 0;
        float maxHp = baseHealth != null ? baseHealth.maxHealth : 0;

        hudText.text = $"Wave: {wave}\nGold: {gold}\nBase HP: {hp}/{maxHp}";
    }
}