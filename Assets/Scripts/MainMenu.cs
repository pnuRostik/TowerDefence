using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenu : MonoBehaviour
{
    public void ClickPlay()
    {
        PlayerPrefs.SetInt("TwoPlayerMode", 0);
        PlayerPrefs.Save();
        SceneManager.LoadScene("GameScene");
    }

    public void ClickPlayTwoPlayers()
    {
        PlayerPrefs.SetInt("TwoPlayerMode", 1);
        PlayerPrefs.Save();
        SceneManager.LoadScene("GameScene");
    }
}
