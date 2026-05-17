using System.Collections.Generic;
using UnityEngine;

public class ProjectileManager : MonoBehaviour
{
    public static ProjectileManager Instance { get; private set; }

    private Dictionary<GameObject, Queue<GameObject>> poolDictionary = new Dictionary<GameObject, Queue<GameObject>>();
    private Transform poolRoot;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        EnsurePoolRoot();
    }

    private void EnsurePoolRoot()
    {
        if (poolRoot != null) return;

        var poolObject = new GameObject("ProjectilePool");
        poolRoot = poolObject.transform;
    }

    public GameObject GetProjectile(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        if (prefab == null) return null;

        EnsurePoolRoot();

        if (!poolDictionary.ContainsKey(prefab))
            poolDictionary[prefab] = new Queue<GameObject>();

        Queue<GameObject> queue = poolDictionary[prefab];

        int count = queue.Count;
        for (int i = 0; i < count; i++)
        {
            GameObject obj = queue.Dequeue();
            queue.Enqueue(obj);

            if (!obj.activeInHierarchy)
            {
                obj.transform.SetParent(poolRoot, false);
                obj.transform.position = position;
                obj.transform.rotation = rotation;
                obj.SetActive(true);
                return obj;
            }
        }

        GameObject newObj = Instantiate(prefab, position, rotation, poolRoot);
        queue.Enqueue(newObj);
        return newObj;
    }
}