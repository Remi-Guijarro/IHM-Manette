using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEditor;
using UnityEngine;

public class PlayerController2D : MonoBehaviour
{
    [SerializeField, Tooltip("Maximum speed in u/s.")] 
    float speed = 10f;

    [SerializeField, Tooltip("Grounded acceleration when the player moves.")]
    float acceleration = 50f;

    [SerializeField, Tooltip("Grounded deceleration when the player does not input movement.")]
    float deceleration = 80f;

    Vector2 velocity;

    // Cached variables
    Collider2D collider;

    void Awake()
    {
        this.collider = GetComponent<Collider2D>();
    }

    void Update()
    {
        ComputeXVelocity(); 
        Move();
    }

    /// <summary>
    /// Calculates the x velocity to be applied.
    /// </summary>
    private void ComputeXVelocity()
    {
        float xAxis = Input.GetAxis("Joystick X");
        print(xAxis);
        velocity.x = Mathf.MoveTowards(velocity.x, speed * xAxis, acceleration * Time.deltaTime);
    }

    /// <summary>
    /// Uses velocity to translate player.
    /// </summary>
    private void Move()
    {
        transform.Translate(this.velocity * Time.deltaTime);
    }
}
