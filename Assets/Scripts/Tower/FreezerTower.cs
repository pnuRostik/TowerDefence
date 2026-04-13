using UnityEngine;

public class FreezerTower : BaseTower
{
    protected override void Attack(Enemy target)
    {
       enemiesInRange.RemoveAll(e => e == null);
       
        foreach (Enemy enemy in enemiesInRange)
        {
            Debug.Log("slow");
            enemy.ApplySlow(0.5f, 2, data.damage); // in %
        }
    }
}
