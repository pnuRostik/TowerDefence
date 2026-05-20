using UnityEngine;
using TMPro;

public class GameHUD : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI waveText;
    [SerializeField] private TextMeshProUGUI goldText;
    [SerializeField] private TextMeshProUGUI hpText;

    private EnemySpawner spawner;
    private Health baseHealth;

    void Awake()
    {
        spawner = Object.FindAnyObjectByType<EnemySpawner>();
    }

    void Start()
    {
        if (EconomyManager.Instance != null)
        {
            EconomyManager.Instance.OnGoldChanged += HandleGoldChanged;
        }

        if (spawner != null)
        {
            spawner.OnWaveStarted += HandleWaveStarted;
        }

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
        int gold = EconomyManager.Instance != null ? EconomyManager.Instance.gold : 0;
        int wave = GameManager.Instance != null ? GameManager.Instance.CurrentWave : 0;
        int totalWaves = GameManager.Instance != null ? GameManager.Instance.TotalWaves : 0;
        float hp = baseHealth != null ? baseHealth.health : 0;

        if (goldText != null) goldText.text = gold.ToString();
        if (waveText != null) waveText.text = $"{wave}/{totalWaves}";
        if (hpText != null) hpText.text = ((int)hp).ToString();
    }
}
