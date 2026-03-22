using System;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private Path currentPathInstance;
    private GameObject[] path;
    private int currentIndex = 0;
    private Vector3 _targetPosition;

    private void Awake()
    {
        currentPathInstance = GameObject.Find("Path").GetComponent<Path>();
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

        float relativeDisatance = (transform.position - _targetPosition).magnitude;

        if (relativeDisatance < 0.1f)
        {
            currentIndex++;

            if (currentIndex >= path.Length)
            {
                gameObject.SetActive(false);
            }
        }

    }
}
