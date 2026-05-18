using UnityEngine;

public class MageTower : BaseTower
{
    protected override void Attack(Enemy target)
    {
        enemiesInRange.RemoveAll(e => e == null || !e.gameObject.activeSelf);
       
        var targets = new System.Collections.Generic.List<Enemy>(enemiesInRange);
        foreach (Enemy enemy in targets)
        {
            if (enemy != null && enemy.gameObject.activeSelf)
            {
                if (Vector2.Distance(transform.position, enemy.transform.position) <= data.range)
                {
                    enemy.TakeDamage(data.damage); 
                }
            }
        }
    }

    protected override void PlayShootSound() { }
}


