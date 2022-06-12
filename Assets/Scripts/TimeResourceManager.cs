using System;
using BeanLib.References;
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
    /// Invoked when time is changed from any source.
    /// This includes time being added, removed, and drained.
    /// </summary>
    public event Action<float> TimeChanged;

    /// <summary>
    /// Gets the initial amount of time the player should hold.
    /// </summary>
    [field: SerializeField]
    public float MaxTime { get; private set; }

    /// <summary>
    /// Gets the amount of time the player has left.
    /// A value of 1 is equal to 1 second.
    /// </summary>
    [field: SerializeField]
    public float Time { get; private set; }

    /// <summary>
    /// Adds time to the store.
    /// </summary>
    /// <param name="amount">The amount of time to add.</param>
    public void Add(float amount)
    {
        amount = ClampAmount(amount);
        Time += amount;
        TimeAdded?.Invoke(amount);
        TimeChanged?.Invoke(amount);
    }

    /// <summary>
    /// Drains time from the store.
    /// </summary>
    /// <param name="amount">The amount of time to drain.</param>
    public void Drain(float amount)
    {
        amount = ClampAmount(amount);
        Time -= amount;
        TimeRemoved?.Invoke(amount);
        TimeChanged?.Invoke(amount);
    }

    private float ClampAmount(float amount)
    {
        float diff = MaxTime - Time;

        return Mathf.Min(diff, amount);
    }

    private void Awake()
    {
        ReferenceStore.ReplaceReference(this);

        Time = MaxTime;
    }

    private void Update()
    {
        // drain time by standardDrain per second
        float drain = standardDrain * UnityEngine.Time.deltaTime;
        Time -= drain;

        // invoke event
        TimeChanged?.Invoke(drain);
    }
}