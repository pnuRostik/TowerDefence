using System;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private bool isGhost = false;
    private float defaultSpeed;

    [SerializeField] private int damage = 10;

    public float distanceTravelled;
    private GameObject[] path;
    private int currentIndex = 0;
    private Vector3 _targetPosition;
    private Vector3 _lastPosition;
    public Health myHealth;
    private EnemySpawner spawner;
    private int goldReward = 10;

    private float slowTimer = 0f;
    private SpriteRenderer spriteRenderer;
    private Color originalColor = Color.white;

    private void Awake()
    {
        myHealth.OnHealthChanged.AddListener(CheckDeath);
        spriteRenderer = GetComponent<SpriteRenderer>();
        spawner = UnityEngine.Object.FindAnyObjectByType<EnemySpawner>();

        defaultSpeed = moveSpeed;
    }

    public void InitializePath(GameObject[] assignedPath)
    {
        path = assignedPath;
        currentIndex = 0;
        distanceTravelled = 0f;

        

        if (path != null && path.Length > 0)
        {
            transform.position = path[0].transform.position;
            _lastPosition = transform.position;
        }
    }
   

    private void CheckDeath(float current, float max)
    {
        if (path == null || !gameObject.activeSelf) return;

        if (current <= 0)
        {
            if (EconomyManager.Instance != null)
            {
                EconomyManager.Instance.AddGold(goldReward);
            }
            DeactivateEnemy();
        }
    }

    public void SetGoldReward(int amount)
    {
        goldReward = amount;
    }

    void OnEnable()
    {
        moveSpeed = defaultSpeed;
        slowTimer = 0f;
        if (myHealth != null) myHealth.ResetHealth(); 
        if (spriteRenderer != null) spriteRenderer.color = originalColor;
    }

    void Update()
    {
        if (GameManager.Instance != null &&
            (GameManager.Instance.CurrentState == GameState.Victory ||
             GameManager.Instance.CurrentState == GameState.Loss))
            return;

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
                if (tower != null)
                {
                    var health = tower.GetComponent<Health>();
                    if (health != null) health.TakeDamage(damage);
                    if(health.health <= 0)
                    {
                        GameManager.Instance.TriggerLoss();
                    }
                }
                DeactivateEnemy();
            }
        }

    }

    public void TakeDamage(float damage)
    {
        myHealth.TakeDamage(damage);
    }

    private void DeactivateEnemy()
    {
        gameObject.SetActive(false); 

        if (spawner != null)
        {
            spawner.EnemyDestroyed(); 
        }
    }


    public void ApplySlow(float slowFactor, float duration, float damage)
    {
        if (isGhost) return;
        
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
