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

        Collider2D[] hits;
        DetectCollisions(out hits);
        Move();
        ResolveCollisions(hits);
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
