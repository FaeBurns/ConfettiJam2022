using System.Collections;
using BeanLib.References;
using UnityEngine;

/// <summary>
/// Component responsible for handling the player's movement.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerPositionReporter))]
public class PlayerMovement : ReferenceResolvedBehaviour
{
    private Vector2 velToMove;
    private PlayerMovementState movementState;

    [AutoReference] private TimeResourceManager timeManager = null;
    [BindComponent] private Rigidbody2D rb = null;
    [BindMultiComponent(Child = true)] private TrailRenderer[] trailRenderers = null;

    [Header("Children")]
    [SerializeField]
    private Transform moveDirectionParent;

    [Header("Fields")]
    [SerializeField] private float speed = 0.25f;
    [SerializeField] private float dashSpeed = 1f;
    [SerializeField] private float dashMultiplierBonus = 1f;
    [SerializeField] private float dashTime = 0.5f;
    [SerializeField] private float timeCost = 1f;
    [SerializeField] private float velDrain = 0.95f;

    /// <summary>
    /// Gets the current <see cref="PlayerMovementState"/>.
    /// </summary>
    public PlayerMovementState MovementState
    {
        get => movementState;
        set
        {
            if (movementState != value)
            {
                // only show trail when dashing
                trailRenderers.Execute((trail) => trail.emitting = value == PlayerMovementState.Dash);

                // switch on old state
                switch (movementState)
                {
                    case PlayerMovementState.Dash:
                        velToMove /= dashSpeed;
                        velToMove *= speed;
                        break;
                }

                movementState = value;
            }
        }
    }

    private void Update()
    {
        if (MovementState == PlayerMovementState.Dead)
        {
            return;
        }

        Vector2 rawInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        if (MovementState == PlayerMovementState.Normal)
        {
            velToMove += 0.1f * speed * rawInput.normalized * Time.deltaTime;
            velToMove = Mathf.Min(velToMove.magnitude, 1) * velToMove.normalized;

            // if there is any input at all
            if (rawInput.sqrMagnitude > 0f)
            {
                velToMove = rawInput.normalized * speed;
            }
            else
            {
                velToMove *= velDrain;
            }
        }

        if (Input.GetButtonDown("Dash") && MovementState == PlayerMovementState.Normal)
        {
            // get dash velocity
            velToMove = rawInput.normalized * dashSpeed;

            // only dash if there is any velocity
            if (velToMove.sqrMagnitude > 0f)
            {
                StartCoroutine(Dash());
            }
        }

        UpdateForwardDirectionTransform();
    }

    private void FixedUpdate()
    {
        switch (MovementState)
        {
            // same on normal and dash
            case PlayerMovementState.Normal:
            case PlayerMovementState.Dash:
                rb.MovePosition(rb.position + velToMove);
                break;

            // else do nothing
            default:
                break;
        }
    }

    private IEnumerator Dash()
    {
        // enter dash state
        MovementState = PlayerMovementState.Dash;

        timeManager.AddMultiplier(dashMultiplierBonus);

        // drain time
        timeManager.Drain(timeCost);

        float endTime = Time.time + dashTime;

        while (endTime > Time.time)
        {
            if (MovementState != PlayerMovementState.Dash)
            {
                // exit if not dashing
                // can occur if player dies during dash
                yield break;
            }

            // wait a frame
            yield return null;
        }

        // reset movement state
        MovementState = PlayerMovementState.Normal;
    }

    private void UpdateForwardDirectionTransform()
    {
        // only update if currently moving
        if (velToMove.sqrMagnitude > 0)
        {
            // can't find the correct variation to use here - have to do some wrangling.
            Vector3 convertedForwardVector = new Vector3(velToMove.normalized.x, 0, velToMove.normalized.y);

            // fix for console getting spammed with error messages
            if (convertedForwardVector == Vector3.zero)
            {
                convertedForwardVector.y = 0.01f;
            }

            // set forward vector
            moveDirectionParent.forward = convertedForwardVector;

            // get as rotation
            Vector3 rotation = moveDirectionParent.rotation.eulerAngles;

            // move value in Y to Z - fixes 2d to 3d conversion
            moveDirectionParent.rotation = Quaternion.Euler(0, 0, -rotation.y);
        }
    }
}