using UnityEngine;
using UnityEngine.SceneManagement; 

public class DeathScreenController : MonoBehaviour
{
    
    public void RestartLevel()
    {

        SceneManager.LoadScene("SampleScene"); 

      
    }
   
    public void QuitToMenu()
    { 
        SceneManager.LoadScene("Main Menu"); 
    }
}
