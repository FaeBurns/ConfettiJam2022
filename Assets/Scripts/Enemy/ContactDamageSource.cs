using System;

/// <summary>
/// The types of contact that can cause damage.
/// </summary>
[Flags]
public enum ContactDamageSource
{
    /// <summary>
    /// Damage caused by colliding.
    /// </summary>
    Collide,

    /// <summary>
    /// Damage caused by overlapping.
    /// </summary>
    Trigger,
}