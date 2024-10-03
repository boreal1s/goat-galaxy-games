using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InputMap))]
public class ThirdPersonCameraController : MonoBehaviour
{
    [Header("References")]
    public Transform player;

    [SerializeField] 
    CinemachineFreeLook aimCamera;

    [SerializeField]
    private PlayerSettings playerSettings;

    [SerializeField]
    private InputMap inputs;

    private void Awake()
    {
        inputs = GetComponent<InputMap>();
    }

    private void Update()
    {
        #region Aim mode control
        if (inputs.aim)
        {
            aimCamera.gameObject.SetActive(true);
        } else
        {
            aimCamera.gameObject.SetActive(false);
        }
        #endregion

        #region Rotate player with camera
        Vector3 viewDirection = player.position - new Vector3(transform.position.x, player.position.y, transform.position.z);
        player.forward = viewDirection.normalized;
        #endregion
    }

    void OnApplicationFocus(bool focus)
    {
        if (focus)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }
}
