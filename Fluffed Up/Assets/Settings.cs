using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Settings : MonoBehaviour
{
    PlayerSettings ps;

    public void Start()
    {
        ps = GameObject.Find("PlayerSettings").GetComponent<PlayerSettings>();
    }

    public void BackToMainMenu()
    {
        ps.UpdateValues();
        SceneManager.LoadScene("Main Menu");
    }
}
