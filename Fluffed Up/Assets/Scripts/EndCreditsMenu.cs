using UnityEngine;
using UnityEngine.SceneManagement; 

public class EndCreditsMenu : MonoBehaviour
{
    public void QuitToMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }
}
