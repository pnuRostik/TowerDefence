using UnityEngine;

public class RangeIndicator : MonoBehaviour
{
    private BaseTower parentTower;

    private void Start()
    {
        gameObject.SetActive(false);
        UpdateScale();
    }

    public static void ShowFor(BaseTower tower)
    {
        HideAll();
        if (tower == null) return;

        RangeIndicator indicator = tower.GetComponentInChildren<RangeIndicator>(true);
        if (indicator == null) return;

        indicator.UpdateScale();
        indicator.gameObject.SetActive(true);
    }

    public static void HideAll()
    {
        foreach (RangeIndicator indicator in FindObjectsByType<RangeIndicator>(FindObjectsSortMode.None))
            indicator.gameObject.SetActive(false);
    }

    private void UpdateScale()
    {
        if (parentTower == null) parentTower = GetComponentInParent<BaseTower>();
        
        if (parentTower != null && parentTower.data != null)
        {
            float range = parentTower.data.range;
            transform.localScale = new Vector3(range * 2, range * 2, 1);
            transform.localPosition = new Vector3(0, 0, 0.1f);
        }
    }
}


