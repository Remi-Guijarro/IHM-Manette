using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float speed = 5f;
    [SerializeField] float sprintSpeed = 30f;

    void Update()
    {
        float xAxis = Input.GetAxis("Joystick X");
        if (Input.GetButton("Sprint"))
        {
            Move(xAxis, sprintSpeed);
        } else
        {
            Move(xAxis, speed);
        }        
    }

    private void Move(float xAxis, float desiredSpeed)
    {
        transform.position += new Vector3(xAxis, 0f) * Time.deltaTime * desiredSpeed;
    }
}
