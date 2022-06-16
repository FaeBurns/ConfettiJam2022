using BeanLib.References;
using UnityEngine;

/// <summary>
/// Component repsonsible for handling the movement of a shotgun projectile.
/// </summary>
public class ShotgunPelletProjectile : ReferenceResolvedBehaviour
{
    private bool destroying = false;

    [BindComponent] private Rigidbody2D rb;

    [SerializeField] private LayerMask layerMask;

    /// <summary>
    /// Gets or Sets the velocity the pellet should move with.
    /// </summary>
    public Vector2 Velocity { get; set; }

    private void FixedUpdate()
    {
        if (destroying)
        {
            Destroy(gameObject);
            return;
        }

        // check if next move cycle will collide with something.
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Velocity.normalized, Velocity.magnitude, layerMask);

        if (hit)
        {
            destroying = true;

            rb.MovePosition(rb.position + (Velocity.normalized * hit.distance));

            return;
        }

        rb.MovePosition(rb.position + Velocity);
    }
}