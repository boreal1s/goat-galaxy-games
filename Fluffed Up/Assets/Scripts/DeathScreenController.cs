using UnityEngine;
using UnityEngine.SceneManagement; 

public class DeathScreenController : MonoBehaviour
{
    
    public void RestartLevel()
    {
        string lastScene = PlayerPrefs.GetString("LastPlayedScene", string.Empty);

        if (!string.IsNullOrEmpty(lastScene))
        {
            // Load the last played scene if it exists
            SceneManager.LoadScene(lastScene);
        }
        else
        {
            // Handle the case where no scene is stored (optional)
            Debug.LogError("No last scene found to restart!");
        }
    }
   
    public void QuitToMenu()
    { 
        SceneManager.LoadScene("Main Menu"); 
    }
}
