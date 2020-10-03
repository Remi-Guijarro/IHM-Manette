using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float speed = 5f;
    [SerializeField] float dashDuration = 1f;
    [SerializeField] float dashSpeed = 20f;
    [SerializeField] float dashDistance = 0.01f;
    [SerializeField] float dashIncrement = 0.5f;
    [SerializeField] float sprintSpeed = 30f;
    [SerializeField] float impulse = 10f;
    [SerializeField] float jumpMultiplier = 2f;
    [SerializeField] float fallMultiplier = 3f;
    [SerializeField] float gravity = 9.81f;
    [SerializeField] bool moveWhileDashing = false;

    
    float currentDashTime;
    bool isGrounded = true;
    float yVelocity;
    float groundYPosition; // Store ground y position below the player
    float orientation;
    Queue<Vector3> dashPositions;

    // Cached variables
    Rigidbody2D rigidbody;
    BoxCollider2D collider;
    float playerHeight;

    private void Start()
    {
        this.rigidbody = GetComponent<Rigidbody2D>();
        this.collider = GetComponent<BoxCollider2D>();
        
        this.playerHeight = collider.size.y;
        this.groundYPosition = transform.position.y - playerHeight / 2;
        this.currentDashTime = 0;
        this.orientation = 1f;
        this.dashPositions = new Queue<Vector3>();
    }

    void Update()
    {
        CheckGroundContact();
        Move();
        Jump();
        Dash();
    }

    private void CheckGroundContact()
    {
        RaycastHit2D hit = Physics2D.Raycast(FeetPosition(), Vector2.down);

        if (hit.collider != null)
        {
            Debug.DrawLine(transform.position, hit.point, Color.red);
            if (hit.distance <= Mathf.Epsilon)
            {
                this.isGrounded = true;
                AdjustYPosition();
            }
            else
            {
                this.isGrounded = false;
                this.groundYPosition = hit.point.y;
            }
        }
    }

    private void AdjustYPosition()
    {
        float feetYPosition = FeetPosition().y;
        if (feetYPosition < this.groundYPosition)
        {
            float groundIntersection = this.groundYPosition - feetYPosition;
            transform.position += new Vector3(0f, groundIntersection);
        }
    }

    private Vector2 FeetPosition()
    {
        return new Vector2(transform.position.x, transform.position.y - this.playerHeight / 2f);
    }

    private void Jump()
    {
        if (this.isGrounded && Input.GetButton("Jump"))
        {
            this.yVelocity = this.impulse;
        }
        else if (isGrounded)
        {
            this.yVelocity = 0;
        }
        else
        {
            float yVelocityModifier = this.gravity * Time.deltaTime;
            if (this.yVelocity > 0)
            {
                yVelocityModifier *= this.jumpMultiplier;
            }
            if (this.yVelocity < 0)
            {
                yVelocityModifier *= this.fallMultiplier;
            }
            this.yVelocity -= yVelocityModifier;
        }

        transform.position += transform.up * this.yVelocity * Time.deltaTime;
    }

    private void Move()
    {
        float xAxis = Input.GetAxis("Joystick X");
        this.orientation = xAxis == 0 ? this.orientation : (xAxis / Math.Abs(xAxis));
        if(this.dashPositions.Count <= 0 || moveWhileDashing)
        {
            if (Input.GetButton("Sprint"))
            {
                Move(xAxis, sprintSpeed);
            }
            else
            {
                Move(xAxis, speed);
            }
        }       
    }

    private void Move(float xAxis, float desiredSpeed)
    {
        transform.position += new Vector3(xAxis, 0f) * Time.deltaTime * desiredSpeed;
    }

    private void MoveTo(Vector3 location, float speed)
    {
        transform.position += location * speed * Time.deltaTime;
    }

    private void Dash()
    {
        if (Input.GetButtonDown("Dash")) {
            for (int i = 0; i < (int)(this.dashDuration / dashIncrement); i++)
            {
                this.dashPositions.Enqueue(Dash(this.orientation));
            }
        } if (this.dashPositions.Count > 0) {
            MoveTo(this.dashPositions.Dequeue(), dashSpeed);
        }        
    }

    private Vector3 Dash(float xAxis)
    {
        if (currentDashTime < dashDuration && xAxis != 0) {
            Vector3 moveDirection;
            moveDirection = new Vector3((xAxis * dashDistance) * dashIncrement, 0.0f);
            currentDashTime += dashIncrement;
            return moveDirection;
        } else {
            currentDashTime = 0f;
            return Vector3.zero;
        }
    }
}
 