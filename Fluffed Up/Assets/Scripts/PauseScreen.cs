using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseScreen : MonoBehaviour
{
    private bool isPaused = false;
    private BGMPlayer bgmPlayer;
    [SerializeField] private GameObject pauseScreenUI;

    void Start()
    {
        pauseScreenUI.SetActive(false);
        GameObject bgmObject = GameObject.FindGameObjectWithTag("BGM");
        bgmPlayer = bgmObject.GetComponent<BGMPlayer>();
    }

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
        bgmPlayer.DimAndDull();
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        isPaused = true;
        pauseScreenUI.SetActive(true);
    }

    public void ResumeGame()
    {
        bgmPlayer.LoudAndClear();
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        isPaused = false;
        pauseScreenUI.SetActive(false);
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        isPaused = false;
        pauseScreenUI.SetActive(false);
        SceneManager.LoadScene("SampleScene");
    }

    public void QuitToMenu()
    {
        Time.timeScale = 1f;  
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        isPaused = false;
        pauseScreenUI.SetActive(false);
        SceneManager.LoadScene("Main Menu");
    }
}
