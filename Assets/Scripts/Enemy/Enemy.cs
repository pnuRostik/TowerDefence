using System;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private Path currentPathInstance;
    [SerializeField] private int damage = 10;
    private GameObject[] path;
    private int currentIndex = 0;
    private Vector3 _targetPosition;
    public Health myHealth;

    private void Awake()
    {
        currentPathInstance = GameObject.Find("Path").GetComponent<Path>();
        myHealth.OnHealthChanged.AddListener(CheckDeath);
    }

    private void CheckDeath(int current, int max)
    {
        if (current == 0)
        {
            Destroy(gameObject);
        }
    }

    void OnEnable()
    {
        int randomIndex = UnityEngine.Random.Range(0, 3);
        path = currentPathInstance.GetPath(randomIndex);

        Debug.Log(randomIndex);

        currentIndex = 0;

        if (path != null && path.Length > 0)
        {
            transform.position = path[0].transform.position;
        }
    }

    void Update()
    {

        if (path == null || path.Length == 0) return;

        _targetPosition = path[currentIndex].transform.position;

        transform.position = Vector3.MoveTowards(
            transform.position,
            _targetPosition,
            moveSpeed * Time.deltaTime
        );
        if (UnityEngine.Random.Range(0, 100) < 10)
            myHealth.TakeDamage(1);

        float relativeDisatance = (transform.position - _targetPosition).magnitude;

        if (relativeDisatance < 0.1f)
        {
            currentIndex++;

            if (currentIndex >= path.Length)
            {
                GameObject tower = GameObject.FindWithTag("Tower");
                var health = tower.GetComponent<Health>();
                health.TakeDamage(damage);
                Destroy(gameObject);
            }
        }

    }
}
