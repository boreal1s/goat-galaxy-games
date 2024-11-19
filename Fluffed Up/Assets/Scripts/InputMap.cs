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
    public bool dodge;

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

    public void OnDodge(InputValue value)
    {
        DodgeInput(value.isPressed);
    }

    public void DodgeInput(bool dodgeState)
    {
        dodge = dodgeState;
    }
}
