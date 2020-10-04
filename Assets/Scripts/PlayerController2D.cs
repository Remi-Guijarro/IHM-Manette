﻿using System;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class PlayerController2D : MonoBehaviour
{
    [SerializeField, Tooltip("Maximum speed in u/s.")] 
    float speed = 10f;

    [SerializeField, Tooltip("Grounded acceleration when the player moves.")]
    float groundAcceleration = 50f;

    [SerializeField, Tooltip("Grounded deceleration when the player does not input movement.")]
    float groundDeceleration = 80f;

    [SerializeField, Tooltip("Maximum height the player will jump regardless of gravity.")]
    float jumpHeight = 5f;

    [SerializeField, Tooltip("Gravity applied to the player.")]
    float gravity = -9.81f;

    [SerializeField, Tooltip("Acceleration while in the air.")]
    float airAcceleration = 30f;

    Vector2 velocity;
    bool isGrounded;

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
        if (this.isGrounded)
        {
            this.velocity.y = 0;

            if (Input.GetButtonDown("Jump"))
            {
                this.velocity.y = Mathf.Sqrt(2 * this.jumpHeight * Mathf.Abs(this.gravity));
            }
        }

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
                    if (Vector2.Angle(colliderDistance.normal, Vector2.up) < 90 && this.velocity.y < 0)
                    {
                        this.isGrounded = true;
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
        float xAxis = Input.GetAxis("Joystick X");

        float acceleration = this.isGrounded ? this.groundAcceleration : this.airAcceleration;
        float deceleration = this.isGrounded ? this.groundDeceleration : 0;

        if (xAxis != 0f)
        {
            this.velocity.x = Mathf.MoveTowards(this.velocity.x, this.speed * xAxis, acceleration * Time.deltaTime);
        }
        else
        {
            this.velocity.x = Mathf.MoveTowards(this.velocity.x, 0, deceleration * Time.deltaTime);
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
