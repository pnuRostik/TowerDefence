using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Enemy target;
    private float damage;
    public float speed = 10f;

    public bool isExplosive;
    public float explosionRadius;

    public void Setup(Enemy _target, float _damage, bool _isExplosive, float _explosionRadius)
    {
        target = _target;
        damage = _damage;
        isExplosive = _isExplosive;
        explosionRadius = _explosionRadius;
    }

    void OnDrawGizmos()
    {
        if (!isExplosive) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }

    void Update()
    {

        if (target == null || target.myHealth.health <= 0)
        {
            Destroy(gameObject);
            return;
        }

        transform.position = Vector3.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);

        Vector3 direction = target.transform.position - transform.position;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0, 0, angle);

        if (Vector3.Distance(transform.position, target.transform.position) < 0.2f)
        {
           this.HitTarget();
        }
    }

    void HitTarget()
    {
        if (isExplosive)
        {
            Explode();
        }
        else
        {
            target.TakeDamage(damage);
        }

        Destroy(gameObject);
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
}
