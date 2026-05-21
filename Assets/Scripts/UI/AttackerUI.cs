using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class AttackerUI : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private Button[] pathButtons;
    [SerializeField] private Transform unitButtonContainer;
    [SerializeField] private GameObject unitButtonPrefab;
    [SerializeField] private Button attackButton;
    [SerializeField] private Button resetButton;

    private EnemySpawner spawner;
    private int selectedPathIndex = 0;

    void Awake()
    {
        spawner = Object.FindAnyObjectByType<EnemySpawner>();
        attackButton.onClick.AddListener(OnAttackClicked);
        if (resetButton != null) resetButton.onClick.AddListener(OnResetClicked);
        
        for (int i = 0; i < pathButtons.Length; i++)
        {
            int index = i;
            pathButtons[i].onClick.AddListener(() => SelectPath(index));
        }
        
        if (panel != null) panel.SetActive(false);
    }

    void Start()
    {
        if (GameManager.Instance != null) 
        {
            HandleStateChanged(GameManager.Instance.CurrentState);
        }
    }

    void OnEnable()
    {
        GameManager.OnStateChanged += HandleStateChanged;
    }

    void OnDisable()
    {
        GameManager.OnStateChanged -= HandleStateChanged;
    }

    private void HandleStateChanged(GameState newState)
    {
        if (panel != null) panel.SetActive(newState == GameState.AttackerPlanning);
        if (newState == GameState.AttackerPlanning)
        {
            SetupUnitButtons();
            RefreshPathButtons();
            SelectPath(0);
        }
    }

    private void RefreshPathButtons()
    {
        int maxPath = spawner.GetMaxAllowedPathIndex(GameManager.Instance.CurrentWave);
        for (int i = 0; i < pathButtons.Length; i++)
        {
            pathButtons[i].interactable = (i <= maxPath);
            
            var text = pathButtons[i].GetComponentInChildren<TextMeshProUGUI>();
            if (text != null)
{
                text.color = (i <= maxPath) ? Color.white : new Color(1, 1, 1, 0.3f);
            }
        }
    }

    private void SetupUnitButtons()
    {
        foreach (Transform child in unitButtonContainer) Destroy(child.gameObject);

        int currentWave = GameManager.Instance.CurrentWave;

        foreach (var type in spawner.enemyTypes)
        {
            if (!spawner.IsEnemyAllowedForWave(type, currentWave)) continue;

            GameObject btnObj = Instantiate(unitButtonPrefab, unitButtonContainer);
            var text = btnObj.GetComponentInChildren<TextMeshProUGUI>();
            if (text != null) text.text = $"{type.name}\n({type.cost})";
            
            var btn = btnObj.GetComponent<Button>();
            btn.onClick.AddListener(() => AddUnit(type));
        }
    }

    private void SelectPath(int index)
    {
        selectedPathIndex = index;
        for (int i = 0; i < pathButtons.Length; i++)
        {
            pathButtons[i].image.color = (i == index) ? Color.green : Color.white;
        }
    }

    private void AddUnit(EnemyType type)
    {
        if (spawner.attackerRemainingBudget >= type.cost)
        {
            spawner.attackerRemainingBudget -= type.cost;
            spawner.attackerQueue.Add(new PlannedEnemy { enemyTypeName = type.name, pathIndex = selectedPathIndex });
            
            var hud = Object.FindAnyObjectByType<GameHUD>();
            if (hud != null) hud.UpdateDisplay();
        }
    }

    private void OnResetClicked()
    {
        if (GameManager.Instance.CurrentState == GameState.AttackerPlanning)
        {
            spawner.attackerRemainingBudget = spawner.GetWaveBudget(GameManager.Instance.CurrentWave);
            spawner.attackerQueue.Clear();
            
            var hud = Object.FindAnyObjectByType<GameHUD>();
            if (hud != null) hud.UpdateDisplay();
        }
    }

    private void OnAttackClicked()
    {
        GameManager.Instance.ChangeState(GameState.Battle);
    }
}
