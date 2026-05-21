using UnityEngine;

public class RangeIndicator : MonoBehaviour
{
    private BaseTower parentTower;

    private void Start()
    {
        CacheParentTower();
        UpdateScale();
        gameObject.SetActive(IsAlwaysVisible(parentTower));
    }

    public static bool IsAlwaysVisible(BaseTower tower)
    {
        return tower is MageTower || tower is FreezerTower;
    }

    public static void ShowFor(BaseTower tower)
    {
        HideClickable();

        if (tower == null || IsAlwaysVisible(tower)) return;

        RangeIndicator indicator = tower.GetComponentInChildren<RangeIndicator>(true);
        if (indicator == null) return;

        indicator.UpdateScale();
        indicator.gameObject.SetActive(true);
    }

    public static void HideAll()
    {
        HideClickable();
    }

    private static void HideClickable()
    {
        foreach (RangeIndicator indicator in FindObjectsByType<RangeIndicator>(FindObjectsSortMode.None))
        {
            indicator.CacheParentTower();
            if (!IsAlwaysVisible(indicator.parentTower))
                indicator.gameObject.SetActive(false);
        }
    }

    private void CacheParentTower()
    {
        if (parentTower == null)
            parentTower = GetComponentInParent<BaseTower>();
    }

    private void UpdateScale()
    {
        CacheParentTower();

        if (parentTower != null && parentTower.data != null)
        {
            float range = parentTower.data.range;
            transform.localScale = new Vector3(range * 2, range * 2, 1);
            transform.localPosition = new Vector3(0, 0, 0.1f);
        }
    }
}


