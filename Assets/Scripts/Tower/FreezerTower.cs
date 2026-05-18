using UnityEngine;

public class FreezerTower : BaseTower
{
   protected override void Attack(Enemy target)
    {
        enemiesInRange.RemoveAll(e => e == null || !e.gameObject.activeSelf);
       
        foreach (Enemy enemy in enemiesInRange)
        {
            enemy.ApplySlow(0.5f, 2f, data.damage); 
        }
    }

    protected override void PlayShootSound() { }
}
