using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSettings : MonoBehaviour
{

    public float SENSITIVITY;
    public float FINE_SENSITIVITY;
    public float CAMERA_SIDE;

    private void Start()
    {
        SENSITIVITY = 1.5f;
        FINE_SENSITIVITY = 0.5f;
        CAMERA_SIDE = 1f;
    }
}
