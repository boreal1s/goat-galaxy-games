using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputMap : MonoBehaviour
{
    [Header("Player Input Values")]
    public bool aim;



    public void OnAim(InputValue value)
    {
        AimInput(value.isPressed);
    }

    public void AimInput(bool aimState)
    {
        aim = aimState;
    }
}
