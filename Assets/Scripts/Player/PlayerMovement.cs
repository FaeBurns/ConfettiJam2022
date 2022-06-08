using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BeanLib.References;

/// <summary>
/// Component responsible for handling the player's movement.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : ReferenceResolvedBehaviour
{
    private PlayerMovementState MovementState
    {
        get => _movementState;
        set
        {
            if (_movementState != value)
            {
                _movementState = value;

                // only show trail when dashing
                trailRenderer.emitting = _movementState == PlayerMovementState.Dash;
            }
        }
    }

    private Vector2 velToMove;
    private PlayerMovementState _movementState;

    [AutoReference] private TimeResourceManager timeManager;
    [BindComponent] private Rigidbody2D rb;
    [BindComponent(Child = true)] private TrailRenderer trailRenderer;

    [Header("Fields")]
    [SerializeField] private float speed = 0.25f;
    [SerializeField] private float dashSpeed = 1f;
    [SerializeField] private float dashTime = 0.5f;
    [SerializeField] private float timeCost = 1f;

    private void Update()
    {
        // get both forms of input
        Vector2 input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        Vector2 rawInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        switch (MovementState)
        {
            case PlayerMovementState.Normal:
                // normalize but keep magnitude - stops the player from moving faster when moving diagonally
                velToMove = Mathf.Min(input.magnitude, 1) * input.normalized;

                // if there is any input at all
                if (rawInput.sqrMagnitude > 0f)
                {
                    velToMove = rawInput.normalized;
                }
                velToMove *= speed;
                break;
            case PlayerMovementState.Dash:
                velToMove = rawInput.normalized * dashSpeed;
                break;
            default:
                break;
        }

        if (Input.GetButtonDown("Dash") && MovementState == PlayerMovementState.Normal && velToMove.sqrMagnitude > 0)
        {
            StartCoroutine(Dash());
        }
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
}