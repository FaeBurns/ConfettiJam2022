using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles keeping track of the time resource.
/// </summary>
public class TimeResourceManager : MonoBehaviour
{
    [SerializeField] private float standardDrain = 1f;

    /// <summary>
    /// Invoked when time is added.
    /// </summary>
    public event Action<float> TimeAdded;

    /// <summary>
    /// Invoked when time is drained.
    /// </summary>
    public event Action<float> TimeRemoved;

    /// <summary>
    /// Gets the initial amount of time the player should hold.
    /// </summary>
    [field: SerializeField]
    public float InitialTime { get; private set; }

    /// <summary>
    /// Gets the amount of time the player has left.
    /// A value of 1 is equal to 1 second.
    /// </summary>
    [field: SerializeField]
    public float TimeResource { get; private set; }

    /// <summary>
    /// Adds time to the store.
    /// </summary>
    /// <param name="amount">The amount of time to add.</param>
    public void Add(float amount)
    {
        TimeResource += amount;
        TimeAdded?.Invoke(amount);
    }

    /// <summary>
    /// Drains time from the store.
    /// </summary>
    /// <param name="amount">The amount of time to drain.</param>
    public void Drain(float amount)
    {
        TimeResource -= amount;
        TimeRemoved?.Invoke(amount);
    }

    private void Update()
    {
        // drain time by standardDrain per second
        TimeResource -= standardDrain * Time.deltaTime;
    }
}
