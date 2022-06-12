using System;
using BeanLib.References;
using UnityEngine;

/// <summary>
/// Component responsible for managing damage dealt to the player.
/// </summary>
[RequireComponent(typeof(Damageable))]
[RequireComponent(typeof(PlayerMovement))]
public class PlayerHealth : ReferenceResolvedBehaviour
{
    [AutoReference] private TimeResourceManager timeManager;
    [BindComponent] private PlayerMovement playerMovement;
    [BindComponent] private Damageable damageable;

    /// <inheritdoc/>
    public override void Start()
    {
        base.Start();

        damageable.OnDamage += OnDamaged;
        timeManager.TimeChanged += RecheckTime;

        damageable.MaxHealth = timeManager.MaxTime;
    }

    private void OnDamaged(float amount, GameObject source, DamageType type)
    {
        // immune if dashing
        if (playerMovement.MovementState != PlayerMovementState.Dash)
        {
            timeManager.Drain(amount);
        }
    }

    private void RecheckTime(float obj)
    {
        damageable.Health = timeManager.Time;
    }
}