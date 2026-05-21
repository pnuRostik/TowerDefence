using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class WaveButtonController : MonoBehaviour
{
    private Button button;
    private CanvasGroup canvasGroup;

    void Awake()
    {
        button = GetComponent<Button>();
        canvasGroup = GetComponent<CanvasGroup>();
        button.onClick.AddListener(OnNextWaveClicked);
    }

    void OnEnable()
    {
        GameManager.OnStateChanged += HandleStateChanged;
        
        if (GameManager.Instance != null)
        {
            UpdateButtonVisibility(GameManager.Instance.CurrentState);
        }
    }

    void OnDisable()
    {
        GameManager.OnStateChanged -= HandleStateChanged;
    }

    private void HandleStateChanged(GameState newState)
    {
        UpdateButtonVisibility(newState);
    }

    private void UpdateButtonVisibility(GameState state)
    {
        bool isPreparation = (state == GameState.Preparation);
        
        canvasGroup.alpha = isPreparation ? 1 : 0;
        canvasGroup.interactable = isPreparation;
        canvasGroup.blocksRaycasts = isPreparation;
    }

    private void OnNextWaveClicked()
    {
        if (GameManager.Instance.CurrentState == GameState.Preparation)
        {
            if (GameManager.Instance.IsTwoPlayerMode)
            {
                GameManager.Instance.ChangeState(GameState.AttackerPlanning);
            }
            else
            {
                GameManager.Instance.ChangeState(GameState.Battle);
            }
        }
    }
}