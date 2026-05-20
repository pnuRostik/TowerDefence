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
    public Path pathSystem;
    public int currentBudget = 150;
    public int budgetIncrease = 75;
    public int maxEnemiesPerWave = 50;
    public float spawnInterval = 1.0f;

    private bool isWaveActive = false;
    private int activeEnemyCount = 0;
    public bool IsWaveActive => isWaveActive;
    public int ActiveEnemyCount => activeEnemyCount;
    public bool CanStartNextWave => !isWaveActive && activeEnemyCount <= 0;

    public event System.Action<int> OnWaveStarted;
    private Dictionary<string, Queue<GameObject>> poolDictionary = new Dictionary<string, Queue<GameObject>>();

    private void OnEnable() => GameManager.OnStateChanged += HandleGameStateChanged;
    private void OnDisable() => GameManager.OnStateChanged -= HandleGameStateChanged;


    private void Awake()
    {
        InitializePools();

        if (pathSystem == null)
        {
            pathSystem = GameObject.FindAnyObjectByType<Path>();
        }
    }

    private void InitializePools()
    {
        foreach (var type in enemyTypes)
        {
            poolDictionary[type.name] = new Queue<GameObject>();
        }
    }

    private GameObject GetEnemyFromPool(EnemyType type, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(type.name))
        {
            poolDictionary[type.name] = new Queue<GameObject>();
        }

        Queue<GameObject> queue = poolDictionary[type.name];

        
        foreach (GameObject obj in queue)
        {
            if (obj != null && !obj.activeSelf)
            {
                obj.transform.position = position;
                obj.transform.rotation = rotation;
                obj.SetActive(true); 
                return obj;
            }
        }

      
        GameObject newObj = Instantiate(type.prefab, position, rotation);
        newObj.transform.SetParent(this.transform); 
        queue.Enqueue(newObj);
        return newObj;
    }

    private void HandleGameStateChanged(GameState newState)
    {
        if (newState == GameState.Battle)
        {
            StartWave();
        }
    }
    public void StopAllActivity()
    {
        StopAllCoroutines();
        isWaveActive = false;
    }

    public void EnemyDestroyed()
    {
        activeEnemyCount--;
    
        if (activeEnemyCount <= 0 && !isWaveActive)
        {         
            Debug.Log("Conditions");
            GameManager.Instance.ChangeState(GameState.RoundEnd);
        }
    }

    public void StartWave()
    {
        OnWaveStarted?.Invoke(GameManager.Instance.CurrentWave);
        List<EnemyType> waveEnemies = GenerateWave();
        StartCoroutine(SpawnWaveRoutine(waveEnemies));
        
        currentBudget += budgetIncrease;
    }

    private List<EnemyType> GenerateWave()
    {
        int currentWave = GameManager.Instance.CurrentWave;
        List<EnemyType> waveEnemies = new List<EnemyType>();
        int remainingBudget = GetWaveBudget(currentWave);
        int maxEnemies = GetMaxEnemiesForWave(currentWave);

        List<EnemyType> affordableTypes = new List<EnemyType>();

        while (remainingBudget > 0 && waveEnemies.Count < maxEnemies)
        {
            affordableTypes.Clear();
            foreach (var type in enemyTypes)
            {
                if (type.cost <= remainingBudget && type.prefab != null && IsEnemyAllowedForWave(type, currentWave))
                {
                    affordableTypes.Add(type);
                }
            }

            if (affordableTypes.Count == 0) break;

            EnemyType selected = affordableTypes[Random.Range(0, affordableTypes.Count)];
            waveEnemies.Add(selected);
            remainingBudget -= selected.cost;
        }

        return waveEnemies;
    }

    private float GetSpawnIntervalForWave(int wave)
    {
        return wave switch
        {
            1 => spawnInterval * 1.4f,
            2 => spawnInterval * 1.2f,
            _ => spawnInterval
        };
    }

    private int GetWaveBudget(int wave)
    {
        float multiplier = wave switch
        {
            1 => 0.55f,
            2 => 0.7f,
            3 => 0.85f,
            _ => 1f
        };

        return Mathf.Max(10, Mathf.RoundToInt(currentBudget * multiplier));
    }

    private int GetMaxEnemiesForWave(int wave)
    {
        return wave switch
        {
            1 => 8,
            2 => 12,
            3 => 18,
            _ => maxEnemiesPerWave
        };
    }

    private bool IsEnemyAllowedForWave(EnemyType type, int wave)
    {
        if (wave == 1) return type.name == "Goblin";
        if (wave == 2) return type.name != "Orc";
        return true;
    }

    private IEnumerator SpawnWaveRoutine(List<EnemyType> enemies)
    {
        isWaveActive = true;
        activeEnemyCount = enemies.Count;

        int currentWave = GameManager.Instance.CurrentWave;

        int maxAllowedPathIndex = 0;
        if (currentWave >= 4 && currentWave <= 6) maxAllowedPathIndex = 1;
        else if (currentWave >= 7) maxAllowedPathIndex = 2;
  

        foreach (EnemyType type in enemies)
        {

            int chosenPathIndex = Random.Range(0, maxAllowedPathIndex + 1);
            int spawnPointIndex = (chosenPathIndex < spawnPoints.Length) ? chosenPathIndex : 0;
            Transform spawnPoint = spawnPoints[spawnPointIndex];


            GameObject enemyObj = GetEnemyFromPool(type, spawnPoint.position, Quaternion.identity);
            
            if (enemyObj.TryGetComponent<Enemy>(out var enemyComp))
            {
                enemyComp.SetGoldReward(type.cost);

                GameObject[] assignedPath = pathSystem.GetPath(chosenPathIndex);
                
                enemyComp.InitializePath(assignedPath);
            }
            
            yield return new WaitForSeconds(GetSpawnIntervalForWave(currentWave));
        }


        isWaveActive = false;
       
  
        if (activeEnemyCount <= 0)
        {
            GameManager.Instance.ChangeState(GameState.RoundEnd);
        }
       
    }
}
