
using System;
using UnityEngine;

public class PlayerInputManager : MonoBehaviour
{
    const string JoystickXAxisName = "Joystick X";
    const string JoystickYAxisName = "Joystick Y";
    const string DPadXAxisName = "DPad X";
    const string DPadYAxisName = "DPad Y";
    const string KeyboardXAxisName = "Keyboard X";
    const string KeyboardYAxisName = "Keyboard Y";

    const string JumpName = "Jump";
    const string SprintName = "Sprint";

    public float HorizontalAxis()
    {
        float axisValue;

        if (Input.GetAxis(JoystickXAxisName) != 0f)
        {
            axisValue = Input.GetAxis(JoystickXAxisName);
        }
        else if (Input.GetAxisRaw(DPadXAxisName) != 0f)
        {
            axisValue = Input.GetAxisRaw(DPadXAxisName);
        }
        else
        {
            axisValue = Input.GetAxisRaw(KeyboardXAxisName);
        }
        
        return axisValue;
    }

    public float Jump()
    {
        return Input.GetAxis(JumpName);
    }

    public bool JumpReleased()
    {
        return Input.GetButtonUp("Jump");   
    }

    internal bool Sprint()
    {
        return Input.GetButton(SprintName);
    }
}
