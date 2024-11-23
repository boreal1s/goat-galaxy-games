using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputMap : MonoBehaviour
{
    [Header("Player Input Values")]
    public bool aim;
    public bool jump;
    public InputActionReference dodge;

    void Start()
    {
        //dodge.action.started += DodgeDown;
        //dodge.action.canceled += DodgeUp;
    }

    public void OnJump(InputValue value)
    {
        JumpInput(value.isPressed);
    }

    public void JumpInput(bool jumpState)
    {
        jump = jumpState;
    }

    public void OnAim(InputValue value)
    {
        AimInput(value.isPressed);
    }

    public void AimInput(bool aimState)
    {
        aim = aimState;
    }

    public void DodgeDown(InputAction.CallbackContext context)
    {
        Debug.Log("Dodge Pressed");
    }

    public void DodgeUp(InputAction.CallbackContext context)
    {
        Debug.Log("Dodge Released");
    }
}
