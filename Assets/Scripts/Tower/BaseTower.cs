using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class BaseTower : MonoBehaviour
{
    public TowerData data;
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
        enemiesInRange.RemoveAll(e => e == null);
        return enemiesInRange.OrderByDescending(e => e.distanceTravelled).FirstOrDefault();
    }

    protected virtual void Attack(Enemy target){}

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<Enemy>(out Enemy enemy))
        {
            Debug.Log("Entered: " + enemy.name);
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