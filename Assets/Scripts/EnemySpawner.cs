using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject[] enemyPrefabs;
    public Transform[] spawnPoints;
    public float spawnInterval = 1.5f;


    public void Start()
    {
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        while (false)
        {
            int idx = Random.Range(0, enemyPrefabs.Length);
            GameObject prefab = enemyPrefabs[idx];
            idx = Random.Range(0, spawnPoints.Length);
            Transform spawnPoint = spawnPoints[idx];
            GameObject enemy = Instantiate(prefab, spawnPoint.position, Quaternion.identity);
            Debug.Log("spawned at " + spawnPoint.position);

            yield return new WaitForSeconds(spawnInterval);
        }
    }
}
