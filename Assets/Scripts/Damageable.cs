﻿using System;
using UnityEngine;

/// <summary>
/// Interface responsible for common functionality between damageable entities.
/// </summary>
public class Damageable : MonoBehaviour
{
    /// <summary>
    /// Invoked when any damage is taken
    /// </summary>
    public event Action<float> OnDamage;

    /// <summary>
    /// Invoked when fatal damage is taken.
    /// </summary>
    public event Action OnDeath;

    /// <summary>
    /// Gets or Sets the health this object has.
    /// </summary>
    [field: SerializeField]
    public float Health { get; set; }

    /// <summary>
    /// Gets or Sets the maximum health this object can hold.
    /// </summary>
    [field: SerializeField]
    public float MaxHealth { get; set; }

    /// <summary>
    /// Deals the specified amount of damage.
    /// </summary>
    /// <param name="damage">The amount of damage to deal.</param>
    public void DealDamage(float damage)
    {
        Health -= damage;
        OnDamage?.Invoke(damage);

        if (Health <= 0)
        {
            OnDeath?.Invoke();
        }
    }
}