using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    private static EffectManager _instance;
    public static EffectManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = Object.FindAnyObjectByType<EffectManager>();
            }
            return _instance;
        }
    }

    private Dictionary<GameObject, Queue<GameObject>> poolDictionary = new Dictionary<GameObject, Queue<GameObject>>();
    private Transform poolRoot;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        EnsurePoolRoot();
    }

    private void EnsurePoolRoot()
    {
        if (poolRoot != null) return;

        var poolObject = new GameObject("EffectPool");
        poolRoot = poolObject.transform;
    }

    public GameObject GetEffect(GameObject prefab, Vector3 position, Quaternion rotation)
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
