
using UnityEngine;

public class PlayerInputManager : MonoBehaviour
{
    const string JOYSTICK_X_AXIS_NAME = "Joystick X";
    const string JOYSTICK_Y_AXIS_NAME = "Joystick Y";
    const string DPAD_X_AXIS_NAME = "DPad X";
    const string DPAD_Y_AXIS_NAME = "DPad Y";
    const string KEYBOARD_X_AXIS_NAME = "Keyboard X";
    const string KEYBOARD_Y_AXIS_NAME = "Keyboard Y";

    const string JUMP_NAME = "Jump";

    public float HorizontalAxis()
    {
        float axisValue;

        if (Input.GetAxis(JOYSTICK_X_AXIS_NAME) != 0f)
        {
            axisValue = Input.GetAxis(JOYSTICK_X_AXIS_NAME);
        }
        else if (Input.GetAxisRaw(DPAD_X_AXIS_NAME) != 0)
        {
            axisValue = Input.GetAxisRaw(DPAD_X_AXIS_NAME);
        }
        else
        {
            axisValue = Input.GetAxisRaw(KEYBOARD_X_AXIS_NAME);
        }
        
        return axisValue;
    }

    public bool Jump()
    {
        return Input.GetButtonDown(JUMP_NAME);
    }
}
