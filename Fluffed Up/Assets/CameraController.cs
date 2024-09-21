using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    public float mouseSensitivity;
    public GameObject mover;

    private Vector2 turn;

    // Start is called before the first frame update
    void Start()
    {
        mouseSensitivity = 5;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        turn.x += Input.GetAxis("Mouse X") * mouseSensitivity;
        turn.y += Input.GetAxis("Mouse Y") * mouseSensitivity;
        mover.transform.localRotation = Quaternion.Euler(0, turn.x, 0);
    }
}
