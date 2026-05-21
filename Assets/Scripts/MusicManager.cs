using UnityEngine;
using UnityEngine.InputSystem;

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

    [Header("Volume Settings")]
    [Range(0f, 1f)] public float towerShootVolume = 0.4f;

    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource soundSource;

    #if UNITY_WEBGL && !UNITY_EDITOR
        private bool _webglAudioUnlocked = false;

        void Update()
        {
            if (!_webglAudioUnlocked)
            {
                if (CheckInteraction())
                {
                    UnlockAudio();
                }
            }
        }

        private bool CheckInteraction()
        {
            if (Pointer.current != null && Pointer.current.press.wasPressedThisFrame) return true;
            if (Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame) return true;
            return false;
        }

        private void UnlockAudio()
        {
            if (musicSource != null && musicSource.clip != null && !musicSource.isPlaying)
            {
                musicSource.Play();
            }
            _webglAudioUnlocked = true;
        }
    #endif

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
    public void PlayTowerShoot() => soundSource.PlayOneShot(towerShootSound, towerShootVolume);
    }