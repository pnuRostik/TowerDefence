using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenu : MonoBehaviour
{
    public void ClickPlay()
    {
        SceneManager.LoadScene("GameScene");
    }
}
