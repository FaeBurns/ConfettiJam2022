using BeanLib.References;
using UnityEngine;

/// <summary>
/// A base class for common enemy behaviour.
/// </summary>
[RequireComponent(typeof(Damageable))]
public class EnemyBase : ReferenceResolvedBehaviour
{
    /// <summary>
    /// Moves the enemy to the target position.
    /// </summary>
    /// <param name="position">The position to move to.</param>
    public void MoveToPosition(Vector2 position)
    {
    }

    /// <summary>
    /// Called when the player exits the detection range.
    /// </summary>
    public virtual void OnPlayerExitDetectionRange()
    {
    }

    /// <summary>
    /// Called when the player enters the detection range.
    /// </summary>
    /// <param name="playerObject">The player.</param>
    public virtual void OnPlayerEnterDetectionRange(GameObject playerObject)
    {
    }
}