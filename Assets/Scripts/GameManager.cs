using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public enum GameState
{
    Preparation,
    Battle,
    RoundEnd,
    Victory,
    Loss
}

public class GameManager : MonoBehaviour
{
    
    public static GameManager Instance;
    [SerializeField] public GameState CurrentState;
    public static event Action<GameState> OnStateChanged;

    [SerializeField] private int totalWaves = 5; 
 
    public int CurrentWave  = 1; 
    public int TotalWaves => totalWaves;

    private void Awake()
    {
        
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); return; }
        
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        ChangeState(GameState.Preparation);
    }

    public void ChangeState(GameState newState)
    {
        if (CurrentState == newState) return;
       

        switch (CurrentState)
        {
            case GameState.Preparation:
                break;
            case GameState.Battle:            
                break;

        }

  
        CurrentState = newState;

     
        switch (CurrentState)
        {
            case GameState.Preparation:
                
                break;
            case GameState.Battle:
                MusicManager.Instance.PlayFight();
                break;
            case GameState.RoundEnd:
                MusicManager.Instance.PlayMenu();
                CheckRoundConditions();
                break;
            case GameState.Victory:
                MusicManager.Instance.PlayWin();
                MusicManager.Instance.PlayMenu();
                PauseGameplay();
                break;
            case GameState.Loss:
                MusicManager.Instance.PlayLose();
                MusicManager.Instance.PlayMenu();
                PauseGameplay();
                break;
        }

        OnStateChanged?.Invoke(CurrentState);
    }

    private void PauseGameplay()
    {
        Time.timeScale = 0f;

        var spawner = FindAnyObjectByType<EnemySpawner>();
        if (spawner != null)
            spawner.StopAllActivity();
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        Instance = null;
        Destroy(gameObject);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        if (Instance != null)
        {
            Destroy(Instance.gameObject);
            Instance = null;
        }
        MusicManager.Instance.PlayMenu();
        SceneManager.LoadScene("MainMenu");
    }


    private void CheckRoundConditions()
    {
        CurrentWave++;

    
        if (CurrentWave > totalWaves)
        {
            ChangeState(GameState.Victory);
            return;
        }

        ChangeState(GameState.Preparation);
    }
    
    public void TriggerLoss()
    {
        ChangeState(GameState.Loss);
    }
}