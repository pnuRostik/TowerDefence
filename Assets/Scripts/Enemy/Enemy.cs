using System;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 3f;
    private float defaultSpeed;
    [SerializeField] private Path currentPathInstance;
    [SerializeField] private int damage = 10;

    public float distanceTravelled;
    private GameObject[] path;
    private int currentIndex = 0;
    private Vector3 _targetPosition;
    private Vector3 _lastPosition;
    public Health myHealth;

    private float slowTimer = 0f;
    private SpriteRenderer spriteRenderer;
    private Color originalColor = Color.white;

    private void Awake()
    {
        currentPathInstance = GameObject.Find("Path").GetComponent<Path>();
        myHealth.OnHealthChanged.AddListener(CheckDeath);
        spriteRenderer = GetComponent<SpriteRenderer>();

        defaultSpeed = moveSpeed;
    }

    private void CheckDeath(float current, float max)
    {
        if (current == 0)
        {
            Destroy(gameObject);
        }
    }

    void OnEnable()
    {
        moveSpeed = defaultSpeed;
        if(spriteRenderer != null) spriteRenderer.color = originalColor;

        int randomIndex = UnityEngine.Random.Range(0, 3);
        path = currentPathInstance.GetPath(randomIndex);
        _lastPosition = transform.position;
        distanceTravelled = 0f;

        Debug.Log(randomIndex);

        currentIndex = 0;

        if (path != null && path.Length > 0)
        {
            transform.position = path[0].transform.position;
        }
    }

    void Update()
    {
        HandleSlowTimer();

        if (path == null || path.Length == 0) return;

        _targetPosition = path[currentIndex].transform.position;

        transform.position = Vector3.MoveTowards(
            transform.position,
            _targetPosition,
            moveSpeed * Time.deltaTime
        );

  

        float relativeDisatance = (transform.position - _targetPosition).magnitude;
        float stepDistance = Vector3.Distance(transform.position, _lastPosition);
        distanceTravelled += stepDistance;

        _lastPosition = transform.position;

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

    public void TakeDamage(float damage)
    {
        myHealth.TakeDamage(damage);
    }



    public void ApplySlow(float slowFactor, float duration, float damage)
    {
        //// if type ghost skip
        
        slowTimer = duration;
        moveSpeed = defaultSpeed * slowFactor; 
        this.TakeDamage(damage);
        
        if (spriteRenderer != null)
        {
            spriteRenderer.color = new Color(0.5f, 0.7f, 1f);
        }
    }

    private void HandleSlowTimer()
    {
        if (slowTimer > 0)
        {
            slowTimer -= Time.deltaTime;

            if (slowTimer <= 0)
            {
                moveSpeed = defaultSpeed;
                if (spriteRenderer != null)
                {
                    spriteRenderer.color = originalColor;
                }
            }
        }
    }
}
