using System;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class PlayerController2D : MonoBehaviour
{
    [SerializeField, Tooltip("Maximum speed in u/s.")] 
    float speed = 10f;

    [SerializeField, Tooltip("Grounded acceleration when the player moves.")]
    float acceleration = 50f;

    [SerializeField, Tooltip("Grounded deceleration when the player does not input movement.")]
    float deceleration = 80f;

    [SerializeField, Tooltip("Maximum height the player will jump regardless of gravity.")]
    float jumpHeight = 5f;

    [SerializeField, Tooltip("Downward gravity applied to the player.")]
    float gravity = 9.81f;

    Vector2 velocity;

    // Cached variables
    BoxCollider2D collider;

    void Awake()
    {
        this.collider = GetComponent<BoxCollider2D>();
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
        if (Input.GetButtonDown("Jump"))
        {
            velocity.y = Mathf.Sqrt(2 * jumpHeight * Mathf.Abs(Physics2D.gravity.y));
        }
        velocity.y += Physics2D.gravity.y * Time.deltaTime;
    }

    /// <summary>
    /// Resolves the collisions by pushing the player out of each collider.
    /// </summary>
    /// <param name="hits">The colliders in contact with the player.</param>
    private void ResolveCollisions(Collider2D[] hits)
    {
        foreach (Collider2D hit in hits)
        {
            if (hit != this.collider)
            {
                ColliderDistance2D colliderDistance = hit.Distance(this.collider);

                if (colliderDistance.isOverlapped)
                {
                    transform.Translate(colliderDistance.pointA - colliderDistance.pointB);
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
        hits = Physics2D.OverlapBoxAll(transform.position, collider.size, 0);
    }

    /// <summary>
    /// Calculates the x velocity to be applied.
    /// </summary>
    private void ComputeXVelocity()
    {
        float xAxis = Input.GetAxis("Joystick X");
        if (xAxis != 0)
        {
            velocity.x = Mathf.MoveTowards(velocity.x, speed * xAxis, acceleration * Time.deltaTime);
        }
        else
        {
            velocity.x = Mathf.MoveTowards(velocity.x, 0, deceleration * Time.deltaTime);
        }
    }

    /// <summary>
    /// Uses velocity to translate player.
    /// </summary>
    private void Move()
    {
        transform.Translate(this.velocity * Time.deltaTime);
    }
}
