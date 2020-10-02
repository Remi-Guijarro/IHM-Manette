using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float speed = 5f;
    [SerializeField] float impulse = 10f;
    [SerializeField] float jumpMultiplier = 2f;
    [SerializeField] float fallMultiplier = 3f;
    [SerializeField] float gravity = 9.81f;

    bool isGrounded = true;
    float yVelocity;
    float groundYPosition; // Store ground y position below player when jumping

    // Cached variables
    Rigidbody2D rigidbody;
    BoxCollider2D collider;
    float playerHeight;

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        collider = GetComponent<BoxCollider2D>();
        
        playerHeight = collider.size.y;
        groundYPosition = transform.position.y - playerHeight / 2;
    }

    void Update()
    {
        Vector2 playerFeetPosition = new Vector2(transform.position.x, transform.position.y - playerHeight / 2f);
        RaycastHit2D hit = Physics2D.Raycast(playerFeetPosition, Vector2.down);

        if (hit.collider != null)
        {
            Debug.DrawLine(transform.position, hit.point, Color.red);
            if (hit.distance <= Mathf.Epsilon)
            {
                isGrounded = true;
                if (hit.point.y < groundYPosition)
                {
                    print(groundYPosition);
                    print(hit.point.y);
                    float groundIntersection = groundYPosition - hit.point.y;
                    print(groundIntersection);
                    transform.position += new Vector3(0f, groundIntersection);
                }
                    
            }
            else
            {
                isGrounded = false;
                groundYPosition = hit.point.y;
            }
        }

        float xAxis = Input.GetAxis("Joystick X");

        transform.position += new Vector3(xAxis, 0f) * Time.deltaTime * speed;

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

    private void FixedUpdate()
    {

    }
}
