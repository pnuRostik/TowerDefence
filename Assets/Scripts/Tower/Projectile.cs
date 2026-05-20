using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Enemy target;
    private float damage;
    public float speed = 10f;

    public bool isExplosive;
    public float explosionRadius;

    [SerializeField] private GameObject explosionEffectPrefab;
    [SerializeField] private GameObject hitEffectPrefab;

    public void Setup(Enemy _target, float _damage)
{
        target = _target;
        damage = _damage;
    }



    private void OnDisable()
    {
        target = null;
    }

    void OnDrawGizmos()
    {
        if (!isExplosive) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }

    void Update()
    {
        if (target == null || !target.gameObject.activeInHierarchy || target.myHealth.health <= 0)
        {
            if (gameObject.activeSelf) Deactivate();
            return;
        }

        transform.position = Vector3.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);

        Vector3 direction = target.transform.position - transform.position;
        if (direction != Vector3.zero)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }

        if (Vector3.Distance(transform.position, target.transform.position) < 0.2f)
        {
           this.HitTarget();
        }
    }

    void HitTarget()
    {
        if (hitEffectPrefab != null)
        {
            if (EffectManager.Instance != null)
            {
                EffectManager.Instance.GetEffect(hitEffectPrefab, transform.position, Quaternion.identity);
            }
            else
            {
                Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
            }
        }

        if (isExplosive)
{
            Explode();
        }
        else
        {
            target.TakeDamage(damage);
        }

        Deactivate();
    }

    void Explode()
    {
        Collider2D[] objectsInRange = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (Collider2D col in objectsInRange)
        {
            if (col.TryGetComponent<Enemy>(out Enemy enemy))
            {
                enemy.TakeDamage(damage);
            }
        }
    }

    private void Deactivate()
    {
        gameObject.SetActive(false); 
    }
}
