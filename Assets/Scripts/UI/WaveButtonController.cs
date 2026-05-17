using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class WaveButtonController : MonoBehaviour
{
    private Button button;
    private CanvasGroup canvasGroup;
    private EnemySpawner spawner;

    void Awake()
    {
        button = GetComponent<Button>();
        canvasGroup = GetComponent<CanvasGroup>();
        spawner = Object.FindAnyObjectByType<EnemySpawner>();
    }

    void Update()
    {
        if (spawner != null && canvasGroup != null)
        {
            bool canStart = spawner.CanStartNextWave;
            
            // Disappear if wave is in progress or enemies are present
            canvasGroup.alpha = canStart ? 1 : 0;
            canvasGroup.interactable = canStart;
            canvasGroup.blocksRaycasts = canStart;
        }
    }
}
