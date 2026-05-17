using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct EnemyType
{
    public string name;
    public GameObject prefab;
    public int cost;
}

public class EnemySpawner : MonoBehaviour
{
    [Header("Wave Settings")]
    public EnemyType[] enemyTypes;
    public Transform[] spawnPoints;
    public int currentBudget = 200;
    public int budgetIncrease = 100;
    public int maxEnemiesPerWave = 50;
    public float spawnInterval = 1.0f;

    private int waveCount = 0;
    private bool isWaveActive = false;
    private int activeEnemyCount = 0;

    public bool IsWaveActive => isWaveActive;
    public int ActiveEnemyCount => activeEnemyCount;
    public int WaveCount => waveCount;
    public bool CanStartNextWave => !isWaveActive && activeEnemyCount <= 0;

    public event System.Action<int> OnWaveStarted;

    public void EnemyDestroyed()
    {
        activeEnemyCount--;
    }

    public void StartWave()
    {
        if (!CanStartNextWave) return;
        
        waveCount++;
        OnWaveStarted?.Invoke(waveCount);
        List<EnemyType> waveEnemies = GenerateWave();
        StartCoroutine(SpawnWaveRoutine(waveEnemies));
        
        currentBudget += budgetIncrease;
    }

    private List<EnemyType> GenerateWave()
    {
        List<EnemyType> wave = new List<EnemyType>();
        int remainingBudget = currentBudget;

        // Create a list of affordable enemies
        List<EnemyType> affordableTypes = new List<EnemyType>();
        
        while (remainingBudget > 0 && wave.Count < maxEnemiesPerWave)
        {
            affordableTypes.Clear();
            foreach (var type in enemyTypes)
            {
                if (type.cost <= remainingBudget && type.prefab != null)
                {
                    affordableTypes.Add(type);
                }
            }

            if (affordableTypes.Count == 0) break;

            EnemyType selected = affordableTypes[Random.Range(0, affordableTypes.Count)];
            wave.Add(selected);
            remainingBudget -= selected.cost;
        }

        return wave;
    }

    private IEnumerator SpawnWaveRoutine(List<EnemyType> enemies)
    {
        isWaveActive = true;
        activeEnemyCount = enemies.Count;
        Debug.Log($"Starting Wave {waveCount} with {enemies.Count} enemies.");

        foreach (EnemyType type in enemies)
        {
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            GameObject enemyObj = Instantiate(type.prefab, spawnPoint.position, Quaternion.identity);
            
            if (enemyObj.TryGetComponent<Enemy>(out var enemyComp))
            {
                enemyComp.SetGoldReward(type.cost);
            }
            
            yield return new WaitForSeconds(spawnInterval);
        }

        isWaveActive = false;
        Debug.Log($"Wave {waveCount} spawning complete.");
    }
}
