using UnityEngine;

public class GameManager : MonoBehaviour
{

    public void RestartLevel()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }

    public void ExitGame()
    {
        UnityEngine.Application.Quit();
    }
}
