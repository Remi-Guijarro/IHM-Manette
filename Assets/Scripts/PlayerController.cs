using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float speed = 5f;

    // Update is called once per frame
    void Update()
    {
        float xAxis = Input.GetAxis("Joystick X");

        transform.position += new Vector3(xAxis, 0f) * Time.deltaTime * speed;
    }
}
