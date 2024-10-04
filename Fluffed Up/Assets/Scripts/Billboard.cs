using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    public Transform targetCamera;

    void Start()
    {
        // Find the camera by name in the scene
        targetCamera = GameObject.Find("PlayerFollowCamera").transform;

        if (targetCamera == null)
        {
            Debug.LogError("Camera 'PlayerFollowCamera' not found!");
        }
    }

    void LateUpdate()
    {
        if (targetCamera != null)
        {
            // Makes sure enemy health bars follow the camera
            transform.LookAt(targetCamera.position);
            transform.Rotate(0, 180, 0); // Optional: Flip the health bar
        }
    }
}
