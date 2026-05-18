using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    public AudioClip menuMusic;
    public AudioClip fightMusic;

    public AudioClip hoverSound;
    public AudioClip clickSound;
    public AudioClip winSound;
    public AudioClip loseSound;
    public AudioClip enemyDieSound;
    public AudioClip enemyReachTowerSound;
    public AudioClip towerShootSound;

    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource soundSource;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        PlayMenu();
    }

    public void PlayMenu()
    {
        Play(menuMusic);
    }

    public void PlayFight()
    {
        Play(fightMusic);
    }

    void Play(AudioClip clip)
    {
        if (musicSource.clip == clip)
            return;

        musicSource.clip = clip;
        musicSource.Play();
    }

    public void PlayHover() => soundSource.PlayOneShot(hoverSound);
    public void PlayClick() => soundSource.PlayOneShot(clickSound);
    public void PlayWin() => soundSource.PlayOneShot(winSound);
    public void PlayLose() => soundSource.PlayOneShot(loseSound);
    public void PlayEnemyDie() => soundSource.PlayOneShot(enemyDieSound);
    public void PlayEnemyReach() => soundSource.PlayOneShot(enemyReachTowerSound);
    public void PlayTowerShoot() => soundSource.PlayOneShot(towerShootSound);
}