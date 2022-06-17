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
    private bool alive = true;

    [AutoReference] private TimeResourceManager timeManager;
    [BindComponent] private PlayerMovement playerMovement;
    [BindComponent] private PlayerAttack playerAttack;
    [BindComponent] private Damageable damageable;

    /// <summary>
    /// Death event.
    /// </summary>
    public event Action OnDeath;

    private void Awake()
    {
        ReferenceStore.ReplaceReference(this);
    }

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
        if (playerMovement.MovementState == PlayerMovementState.Dead)
        {
            return;
        }

        // immune if dashing
        if (playerMovement.MovementState != PlayerMovementState.Dash)
        {
            timeManager.Drain(amount);
        }
    }

    private void RecheckTime(float obj)
    {
        damageable.Health = timeManager.Time;

        if (damageable.Health <= 0 && alive)
        {
            alive = false;

            playerAttack.enabled = false;
            playerMovement.MovementState = PlayerMovementState.Dead;

            OnDeath?.Invoke();
        }
    }
}