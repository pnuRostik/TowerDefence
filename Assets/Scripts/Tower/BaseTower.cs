using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class BaseTower : MonoBehaviour
{
    public TowerData data;
    public GameObject projectilePrefab;
    protected List<Enemy> enemiesInRange = new List<Enemy>();
    protected float lastAttackTime;

    private void Start()
    {
        CircleCollider2D rangeCollider = GetComponent<CircleCollider2D>();
        rangeCollider.radius = data.range;
        rangeCollider.isTrigger = true;
    }

    private void Update()
    {
        if (GameManager.Instance != null &&
            (GameManager.Instance.CurrentState == GameState.Victory ||
             GameManager.Instance.CurrentState == GameState.Loss))
            return;

        if (Time.time >= lastAttackTime + (1f / data.attackSpeed))
        {

            Enemy target = GetBestTarget();
           

            if (target != null || data.attackType == AttackType.Slow) 
            {
                Attack(target);
                lastAttackTime = Time.time;
            }
        }
    }

    protected virtual Enemy GetBestTarget()
    {
        enemiesInRange.RemoveAll(e => e == null || !e.gameObject.activeSelf);
        return enemiesInRange.OrderByDescending(e => e.distanceTravelled).FirstOrDefault();
    }

    protected virtual void Attack(Enemy target)
    {
        if (target == null || projectilePrefab == null) return;

        if (ProjectileManager.Instance == null)
        {
            Debug.LogError("ProjectileManager not found.");
            return;
        }

        PlayShootSound();

        GameObject projectileObj = ProjectileManager.Instance.GetProjectile(projectilePrefab, GetFirePosition(), Quaternion.identity);
        
        if (projectileObj != null && projectileObj.TryGetComponent<Projectile>(out Projectile projectile))
        {
            projectile.Setup(target, data.damage);
        }
        }

        protected virtual void PlayShootSound()
        {
        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.PlayTowerShoot();
        }
        }

    protected virtual Vector3 GetFirePosition()
    {
        float yOffset = 0.5f; 
        return transform.position + new Vector3(0, yOffset, 0);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<Enemy>(out Enemy enemy))
        {
            enemiesInRange.Add(enemy);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent<Enemy>(out Enemy enemy))
        {
            enemiesInRange.Remove(enemy);
        }
    }
    private void OnDrawGizmos()
    {
        if (data == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, data.range);
    }
}