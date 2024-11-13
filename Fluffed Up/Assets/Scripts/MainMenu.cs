using UnityEngine;
using UnityEngine.SceneManagement; 

public class MainMenu : MonoBehaviour
{
    
    public void StartGame()
    {
        SceneManager.LoadScene("Select character");
    }

   
    public void QuitGame()
    {
       Application.Quit();
    }
}
