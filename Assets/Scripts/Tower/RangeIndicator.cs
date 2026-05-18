using UnityEngine;

public class RangeIndicator : MonoBehaviour
{
    private BaseTower parentTower;

    private void Start()
    {
        UpdateScale();
    }

    private void UpdateScale()
    {
        if (parentTower == null) parentTower = GetComponentInParent<BaseTower>();
        
        if (parentTower != null && parentTower.data != null)
        {
            float range = parentTower.data.range;
            // Diameter is 2 * range. Since sprite is 1x1, scale is 2 * range.
            transform.localScale = new Vector3(range * 2, range * 2, 1);
            // Move slightly back in Z to be behind the tower sprite
            transform.localPosition = new Vector3(0, 0, 0.1f);
        }
    }
}


