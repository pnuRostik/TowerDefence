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

[System.Serializable]
public struct PlannedEnemy
{
    public string enemyTypeName;
    public int pathIndex;
}

public class EnemySpawner : MonoBehaviour
{
    [Header("Wave Settings")]
    public EnemyType[] enemyTypes;
    public Transform[] spawnPoints;
    public Path pathSystem;
    public int currentBudget = 150;
    public int budgetIncrease = 150;
    public float spawnInterval = 1.5f;
    [Range(0f, 1f)] public float rewardPercentage = 0.33f;

    public List<PlannedEnemy> attackerQueue = new List<PlannedEnemy>();
    public int attackerRemainingBudget;

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
        else if (newState == GameState.AttackerPlanning)
        {
            attackerRemainingBudget = GetWaveBudget(GameManager.Instance.CurrentWave);
            attackerQueue.Clear();
        }
    }

    public void StartWave()
    {
        OnWaveStarted?.Invoke(GameManager.Instance.CurrentWave);
        
        List<EnemyType> waveEnemies = new List<EnemyType>();
        List<int> wavePaths = new List<int>();

        if (GameManager.Instance.IsTwoPlayerMode)
        {
            foreach (var planned in attackerQueue)
            {
                EnemyType type = System.Array.Find(enemyTypes, t => t.name == planned.enemyTypeName);
                if (type.prefab != null)
                {
                    waveEnemies.Add(type);
                    wavePaths.Add(planned.pathIndex);
                }
            }
        }
        else
        {
            waveEnemies = GenerateWave();
        }

        StartCoroutine(SpawnWaveRoutine(waveEnemies, wavePaths));

        currentBudget += budgetIncrease;
    }

    public int GetWaveBudget(int wave)
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

    private List<EnemyType> GenerateWave()
    {
        int currentWave = GameManager.Instance.CurrentWave;
        List<EnemyType> waveEnemies = new List<EnemyType>();
        int remainingBudget = GetWaveBudget(currentWave);

        List<EnemyType> affordableTypes = new List<EnemyType>();

        while (remainingBudget > 0)
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

    public int GetMaxAllowedPathIndex(int wave)
    {
        if (wave >= 7) return 2;
        if (wave >= 4) return 1;
        return 0;
    }

    private IEnumerator SpawnWaveRoutine(List<EnemyType> enemies, List<int> paths = null)
    {
        yield return null;

        isWaveActive = true;
        activeEnemyCount = enemies.Count;

        int currentWave = GameManager.Instance.CurrentWave;
        int maxAllowedPathIndex = GetMaxAllowedPathIndex(currentWave);

        for (int i = 0; i < enemies.Count; i++)
        {
            EnemyType type = enemies[i];
            int chosenPathIndex;
            
            if (paths != null && i < paths.Count)
            {
                chosenPathIndex = Mathf.Clamp(paths[i], 0, maxAllowedPathIndex);
            }
            else
            {
                chosenPathIndex = Random.Range(0, maxAllowedPathIndex + 1);
            }

            int spawnPointIndex = (chosenPathIndex < spawnPoints.Length) ? chosenPathIndex : 0;
            Transform spawnPoint = spawnPoints[spawnPointIndex];

            GameObject enemyObj = GetEnemyFromPool(type, spawnPoint.position, Quaternion.identity);

            if (enemyObj.TryGetComponent<Enemy>(out var enemyComp))
            {
                int reward = Mathf.RoundToInt(type.cost * rewardPercentage);
                enemyComp.SetGoldReward(reward);

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
            GameManager.Instance.ChangeState(GameState.RoundEnd);
        }
    }

        private float GetSpawnIntervalForWave(int wave)
        {
        float interval = spawnInterval - (wave - 1) * 0.0928f;
        return Mathf.Max(0.2f, interval);
        }

        public bool IsEnemyAllowedForWave(EnemyType type, int wave)
        {
            if (wave == 1) return type.name == "Goblin";
            if (wave == 2) return type.name != "Orc";
            return true;
        }
        }
