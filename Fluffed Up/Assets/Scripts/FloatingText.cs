using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingText : MonoBehaviour
{

    public float DestroyTime = 3f;
    public Vector3 Offset = new Vector3(0, 2.5f, 0);
    public Vector3 RandomizeIntensity = new Vector3(0.5f, 0, 0);

    public Transform targetCamera;

    // Start is called before the first frame update
    void Start()
    {

        // Find the camera by name in the scene
        targetCamera = GameObject.Find("PlayerFollowCamera").transform;

        if (targetCamera == null)
        {
            Debug.LogError("Camera 'PlayerFollowCamera' not found!");
        }

        Destroy(gameObject, DestroyTime);

        transform.localPosition += Offset;
        transform.localPosition += new Vector3(Random.Range(-RandomizeIntensity.x, RandomizeIntensity.x),
        Random.Range(RandomizeIntensity.y, RandomizeIntensity.y),
        Random.Range(RandomizeIntensity.z, RandomizeIntensity.z));
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
