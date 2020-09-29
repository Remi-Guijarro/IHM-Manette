using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float speed = 5f;

    [SerializeField] float sprintSpeed = 30f;

    // Update is called once per frame
    void Update()
    {
        float xAxis = Input.GetAxis("Joystick X");
        if (Input.GetButton("Sprint"))
        {
            transform.position += new Vector3(xAxis, 0f) * Time.deltaTime * sprintSpeed;
        } else
        {
            transform.position += new Vector3(xAxis, 0f) * Time.deltaTime * speed;
        }        
    }
}
