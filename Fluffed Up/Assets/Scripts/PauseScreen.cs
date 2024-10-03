using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseScreen : MonoBehaviour
{
    private bool isPaused = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    void PauseGame()
    {
        Time.timeScale = 0f;
        isPaused = true;
        SceneManager.LoadScene("PauseScreen", LoadSceneMode.Additive);
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        isPaused = false;
        SceneManager.UnloadSceneAsync("PauseScreen");
    }

    public void QuitToMenu()
    {
        Time.timeScale = 1f;  
        SceneManager.LoadScene("Main Menu"); 
    }
}
