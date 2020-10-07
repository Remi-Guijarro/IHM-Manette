using System;
using System.Collections;
using System.Linq.Expressions;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(PlayerInputManager))]
public class PlayerController2D : MonoBehaviour
{
    [SerializeField, Tooltip("Maximum walking speed in u/s.")] 
    float walkingSpeed = 10f;

    [SerializeField, Tooltip("Maximum walking speed in u/s.")]
    float sprintingSpeed = 15f;

    [SerializeField, Tooltip("Dash duration in seconds.")]
    float dashDuration = .25f;

    [SerializeField, Tooltip("Dash speed in u/s.")]
    float dashSpeed = 30f;

    [SerializeField]
    bool dashJumpingAllowed = true;

    [SerializeField, Tooltip("Grounded acceleration when the player moves.")]
    float groundAcceleration = 50f;

    [SerializeField, Tooltip("Grounded deceleration when the player does not input movement.")]
    float groundDeceleration = 80f;

    [SerializeField, Tooltip("Maximum height the player will jump regardless of gravity and on long button press.")]
    float jumpHeight = 5f;

    [SerializeField, Tooltip("Gravity applied to the player. Negative = downward gravity. Positive = upward gravity.")]
    float gravity = -9.81f;

    [SerializeField, Tooltip("Acceleration while in the air.")]
    float airAcceleration = 30f;

    Vector2 velocity;
    IEnumerator dashCoroutine = null;
    bool isGrounded;
    private bool IsDashing
    {
        get
        {
            return dashCoroutine != null;
        }
        set
        {
            if (true == value)
            {
                this.dashCoroutine = DashCoroutine();
                StartCoroutine(dashCoroutine);
            }
            else if (false == value)
            {
                StopCoroutine(this.dashCoroutine);
                this.dashCoroutine = null;
            }
        }
    }
    private bool IsSprinting
    {
        get { return inputManager.Sprint(); }
    }

    enum Orientation
    {
        Left = -1
        , Right = 1
    }

    Orientation orientation = PlayerController2D.Orientation.Right;

    // Cached variables
    BoxCollider2D collider;
    PlayerInputManager inputManager;

    void Awake()
    {
        this.collider = GetComponent<BoxCollider2D>();
        this.inputManager = GetComponent<PlayerInputManager>();
    }

    void Update()
    {
        ComputeXVelocity();
        ComputeYVelocity();

        Collider2D[] hits;
        DetectCollisions(out hits);
        Move();
        ResolveCollisions(hits);
    }

    /// <summary>
    /// Calculates the y velocity to be applied.
    /// 
    /// Y velocity is based on the following formula:
    /// a = (vf² - vo²)/2d
    /// Where : 
    ///     .a is the acceleration, ie the force of gravity;
    ///     .vo is the initial velocity, ie the value we want to solve;
    ///     .vf is the final velocity, ie zero, as there is no motion at the peak of the jump;
    ///     .d is the distance travelled, ie the jump heigh we want to reach.
    /// </summary>
    private void ComputeYVelocity()
    {
        if (this.isGrounded)
        {
            this.velocity.y = 0;

            if (this.inputManager.Jump() != 0f)
            {
                this.velocity.y = Mathf.Sqrt(2 * this.jumpHeight * Mathf.Abs(this.gravity));
            }
        }
        
        if (this.inputManager.JumpReleased() && Vector2.Dot(this.velocity, Vector2.up) > 0)
        {
            float jumpValue = this.inputManager.Jump();
            // Min threshold to avoid making tiny jumps on button quick release
            if (jumpValue < .5f) jumpValue = .5f;
            this.velocity.y *= jumpValue;
        }
        
        if (this.IsDashing) return;
        
        this.velocity.y += this.gravity * Time.deltaTime;
    }

    /// <summary>
    /// Resolves the collisions by pushing the player out of each collider.
    /// </summary>
    /// <param name="hits">The colliders in contact with the player.</param>
    private void ResolveCollisions(Collider2D[] hits)
    {
        this.isGrounded = false;
        foreach (Collider2D hit in hits)
        {
            if (hit != this.collider)
            {
                ColliderDistance2D colliderDistance = hit.Distance(this.collider);
                if (colliderDistance.isOverlapped)
                {
                    transform.Translate(colliderDistance.pointA - colliderDistance.pointB);
                    
                    // Ground is defined as any surface < 90° with the world up
                    if (Vector2.Angle(colliderDistance.normal, Vector2.up) < 90f && this.velocity.y < 0f)
                    {
                        this.isGrounded = true;
                    } else {
                        if(this.velocity.y > 0)
                        {
                            this.velocity.y = 0;
                        } 
                        
                        if (this.dashCoroutine != null && Vector2.Angle(colliderDistance.normal, Vector2.up) != 180f) // Quick fix to avoid stopping dash when collision with ground detected
                        {
                            this.velocity.x = 0;
                            this.IsDashing = false;
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Detect all collisions with player box collider.
    /// </summary>
    /// <param name="hits">The colliders overlapping the box.</param>
    private void DetectCollisions(out Collider2D[] hits)
    {
        hits = Physics2D.OverlapBoxAll(transform.position, this.collider.size, 0);
    }

    /// <summary>
    /// Calculates the x velocity to be applied.
    /// 
    /// Different acceleration and deceleration is applied depending on
    /// whether the player is in the air or not, giving a better jump feeling.
    /// </summary>
    private void ComputeXVelocity()
    {
        float xAxis = inputManager.HorizontalAxis();
        UpdatePlayerOrientation(xAxis);

        float acceleration = this.isGrounded ? this.groundAcceleration : this.airAcceleration;
        float deceleration = this.isGrounded ? this.groundDeceleration : 0;

        float speed = IsSprinting ? this.sprintingSpeed : this.walkingSpeed;

        bool groundCheck = dashJumpingAllowed ? true : this.isGrounded;
        if (groundCheck && !this.IsDashing && inputManager.Dash())
        {
            this.IsDashing = true;
        }

        if (this.IsDashing)
        {
            float orientationValue = (float) this.orientation;
            this.velocity.x = Mathf.Lerp(this.velocity.x, dashSpeed * orientationValue, acceleration * Time.deltaTime);
        }
        else if (xAxis != 0f)
        {
            this.velocity.x = Mathf.MoveTowards(this.velocity.x, speed * xAxis, acceleration * Time.deltaTime);
        }
        else
        {
            this.velocity.x = Mathf.MoveTowards(this.velocity.x, 0, deceleration * Time.deltaTime);
        }
    }

    private void UpdatePlayerOrientation(float xAxis)
    {
        this.orientation = xAxis == 0f ? this.orientation : (Orientation)(xAxis / Math.Abs(xAxis));
        transform.localScale = new Vector3((float) this.orientation, transform.localScale.y, transform.localScale.z);
    }

    private IEnumerator DashCoroutine()
    {
        velocity.y = 0;
        yield return new WaitForSeconds(this.dashDuration);
        IsDashing = false;
    }

    /// <summary>
    /// Uses velocity to translate player.
    /// </summary>
    private void Move()
    {
        transform.Translate(this.velocity * Time.deltaTime);
    }
}
