using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; 

public class SelectChar : MonoBehaviour
{
    
    public static int characterID;
    
    public void StartGameSword()
    {
        characterID = 0;
        SceneManager.LoadScene("SampleScene");
    }

    public void StartGameShooter()
    {
        characterID = 1;
        SceneManager.LoadScene("SampleScene");
    }

  

}
