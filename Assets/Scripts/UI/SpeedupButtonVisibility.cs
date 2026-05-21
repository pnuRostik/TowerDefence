using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class SpeedupButtonVisibility : MonoBehaviour
{
    private CanvasGroup canvasGroup;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    void OnEnable()
    {
        GameManager.OnStateChanged += HandleStateChanged;
        if (GameManager.Instance != null)
        {
            UpdateVisibility(GameManager.Instance.CurrentState);
        }
    }

    void OnDisable()
    {
        GameManager.OnStateChanged -= HandleStateChanged;
    }

    private void HandleStateChanged(GameState newState)
    {
        UpdateVisibility(newState);
    }

    private void UpdateVisibility(GameState state)
    {
        bool isPreparation = (state == GameState.Preparation);
        canvasGroup.alpha = isPreparation ? 1 : 0;
        canvasGroup.interactable = isPreparation;
        canvasGroup.blocksRaycasts = isPreparation;
    }
}