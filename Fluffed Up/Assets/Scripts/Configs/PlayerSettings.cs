using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerSettings : MonoBehaviour
{

    public float SENSITIVITY;
    public float FINE_SENSITIVITY;

    [SerializeField] private Slider mouseSensSlider;
    [SerializeField] private Slider aimSensSlider;

    private void Start()
    {
        SENSITIVITY = 1.5f;
        FINE_SENSITIVITY = 0.5f;

        DontDestroyOnLoad(this);
    }

    private void Update()
    {
        DontDestroyOnLoad(this);
    }

    public void UpdateValues()
    {
        if (mouseSensSlider == null)
        {
            mouseSensSlider = GameObject.Find("StandardSensitivity").GetComponentInChildren<Slider>();
        }

        if (aimSensSlider == null)
        {
            aimSensSlider = GameObject.Find("AimSensitivity").GetComponentInChildren<Slider>();
        }

        SENSITIVITY = mouseSensSlider.value;
        FINE_SENSITIVITY = aimSensSlider.value;
    }
}
