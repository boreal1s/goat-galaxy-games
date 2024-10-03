using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    public Transform targetCamera;

    void LateUpdate()
    {
        // Makes sure enemy health bars follow the camera
        transform.LookAt(transform.position + targetCamera.forward);
    }
}
