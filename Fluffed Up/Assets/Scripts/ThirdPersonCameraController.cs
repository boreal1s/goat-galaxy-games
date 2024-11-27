using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

[RequireComponent(typeof(InputMap))]
public class ThirdPersonCameraController : MonoBehaviour
{
    [Header("References")]

    [SerializeField]
    private PlayerSettings playerSettings;

    [SerializeField]
    private InputMap inputs;

    CinemachineVirtualCamera virtCamera;
    CinemachineVirtualCamera aimCamera;
    GameObject followTarget;

    float yaw = 0f;
    float pitch = 0f;
    float standardFOV = 60f;
    float aimFOV = 40f;
    bool isAiming = false;
    public bool isShopping = false;

    private void Awake()
    {
        inputs = GetComponent<InputMap>();
    }

    private void Start()
    {
        virtCamera = GameObject.Find("PlayerFollowCamera").GetComponent<CinemachineVirtualCamera>();
        aimCamera = GameObject.Find("PlayerAimCamera").GetComponent<CinemachineVirtualCamera>();
        followTarget = GameObject.Find("CameraRoot");

        virtCamera.Follow = followTarget.transform;
        aimCamera.Follow = followTarget.transform;

        virtCamera.m_Lens.FieldOfView = standardFOV;
        aimCamera.m_Lens.FieldOfView = aimFOV;
    }

    private void Update()
    {
        if (!isShopping)
        {
            Debug.Log(playerSettings.SENSITIVITY);
            Debug.Log(playerSettings.FINE_SENSITIVITY);
            pitch += isAiming ? playerSettings.FINE_SENSITIVITY * -Input.GetAxis("Mouse Y") : playerSettings.SENSITIVITY * -Input.GetAxis("Mouse Y");
            yaw += isAiming ? playerSettings.FINE_SENSITIVITY * Input.GetAxis("Mouse X") : playerSettings.SENSITIVITY * Input.GetAxis("Mouse X");
            pitch = Mathf.Clamp(pitch, -90f, 75f);

            followTarget.transform.eulerAngles = new Vector3(pitch, yaw, 0f);

            #region Aim mode control
            if (inputs.aim)
            {
                aimCamera.gameObject.SetActive(true);
                isAiming = true;
            }
            else
            {
                aimCamera.gameObject.SetActive(false);
                isAiming = false;
            }
            #endregion
        }
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
