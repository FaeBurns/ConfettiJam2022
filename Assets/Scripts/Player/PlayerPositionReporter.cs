using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// Component responsible for keeping track of the player's last valid pathfinding position.
/// </summary>
public class PlayerPositionReporter : MonoBehaviour
{
    /// <summary>
    /// Gets the player's last valid pathing position.
    /// </summary>
    // TODO: run checks
    public Vector2 ValidPathPosition { get => (Vector2)transform.position; }
}