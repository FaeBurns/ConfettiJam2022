using System;
using BeanLib.References;
using UnityEngine;

/// <summary>
/// Handles keeping track of the time resource.
/// </summary>
public class TimeResourceManager : MonoBehaviour
{
    private float multiplierDrainStartTime = 0f;

    [SerializeField] private float standardDrain = 1f;
    [SerializeField] private float multiplierDrainDelay = 0.75f;
    [SerializeField] private float multiplierDrain = 1f;
    [SerializeField] private float multiplierMax = 3f;

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
    /// Gets the multiplier applied to add orders.
    /// </summary>
    [field: SerializeField]
    public float AdditionMultiplier { get; private set; } = 1f;

    /// <summary>
    /// Adds time to the store.
    /// </summary>
    /// <param name="amount">The amount of time to add.</param>
    public void Add(float amount, bool affectedByMultiplier = true)
    {
        if (!enabled)
        {
            return;
        }

        amount *= affectedByMultiplier ? AdditionMultiplier : 1;

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
        if (!enabled)
        {
            return;
        }

        amount = ClampAmount(amount);
        Time -= amount;
        TimeRemoved?.Invoke(amount);
        TimeChanged?.Invoke(-amount);
    }

    /// <summary>
    /// Adds to the multiplier.
    /// </summary>
    /// <param name="multiplierAmount">The amount to add.</param>
    public void AddMultiplier(float multiplierAmount)
    {
        AdditionMultiplier = Mathf.Clamp(AdditionMultiplier + multiplierAmount, 1, multiplierMax);

        multiplierDrainStartTime = UnityEngine.Time.time + multiplierDrainDelay;
    }

    /// <summary>
    /// Gets the maximum time from <see cref="GlobalSettings"/>.
    /// </summary>
    public void SetTimeFromSettings()
    {
        float time = GameSettings.Time;

        MaxTime = time;
        Time = time;
    }

    private float ClampAmount(float amount)
    {
        float diff = MaxTime - Time;

        return Mathf.Min(diff, amount);
    }

    private void Awake()
    {
        ReferenceStore.RegisterReference(this);

        SetTimeFromSettings();
    }

    private void Update()
    {
        if (multiplierDrainStartTime <= UnityEngine.Time.time)
        {
            AdditionMultiplier = Mathf.Max(AdditionMultiplier - (multiplierDrain * UnityEngine.Time.deltaTime), 1);
        }

        // drain time by standardDrain per second
        float drain = standardDrain * UnityEngine.Time.deltaTime;
        Time -= drain;

        // invoke event
        TimeChanged?.Invoke(-drain);
    }
}