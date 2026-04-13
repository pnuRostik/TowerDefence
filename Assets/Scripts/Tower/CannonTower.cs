using UnityEngine;

public class CannonTower : BaseTower
{
    public Transform firePoint;
    public GameObject arrowPrefab;


    protected Vector3 GetFirePosition()
    {
        float yOffset = 0.5f; 
        return transform.position + new Vector3(0, yOffset, 0);
    }

    protected override void Attack(Enemy target)
    {
        GameObject projectileObj = Instantiate(arrowPrefab, GetFirePosition(), Quaternion.identity);
        Projectile projectile = projectileObj.GetComponent<Projectile>();
        projectile.Setup(
            target,
            data.damage,     
            false,           
            0f
        );
    }
    
}
