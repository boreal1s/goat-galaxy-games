using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; 

public class SelectChar : MonoBehaviour
{
    

    
    public void StartGameSword()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void StartGameShooter()
    {
        SceneManager.LoadScene("OtherCharacter");
    }

  

}
