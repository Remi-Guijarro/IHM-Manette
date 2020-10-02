using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float speed = 5f;
    [SerializeField] float impulse = 10f;
    [SerializeField] float jumpMultiplier = 2f;
    [SerializeField] float fallMultiplier = 1.5f;
    [SerializeField] float gravity = 9.81f;

    bool isGrounded = true;
    float yVelocity;

    // Cached variables
    Rigidbody2D rigidbody;
    BoxCollider2D collider;

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        collider = GetComponent<BoxCollider2D>();
    }

    void Update()
    {
        float xAxis = Input.GetAxis("Joystick X");

        transform.position += new Vector3(xAxis, 0f) * Time.deltaTime * speed;

    }

    private void FixedUpdate()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, collider.size.y / 2);

        if (hit.collider != null)
        {
            Debug.DrawLine(transform.position, hit.point, Color.red);
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }

        if (isGrounded && Input.GetButton("Jump"))
        {
            yVelocity = impulse; 
        }

        else if (isGrounded)
        {
            yVelocity = 0;
        }
        else
        {
            float yVelocityModifier = gravity * Time.deltaTime;
            if (yVelocity > 0)
            {
                yVelocityModifier *= jumpMultiplier;
            }
            if (yVelocity < 0)
            {
                yVelocityModifier *= fallMultiplier;
            }
            yVelocity -= yVelocityModifier;
        }

        transform.position += transform.up * yVelocity * Time.deltaTime;

    }
}
