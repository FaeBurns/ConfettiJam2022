using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A structure containing data used by a compiled path.
/// </summary>
public struct EnemyPathData
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EnemyPathData"/> struct.
    /// </summary>
    /// <param name="path">The path to contain.</param>
    /// <param name="endGoal">The end goal of the path.</param>
    public EnemyPathData(Stack<Vector2> path, Vector2 endGoal)
    {
        Path = path;
        EndGoal = endGoal;
    }

    /// <summary>
    /// Gets the full remainder of the path.
    /// </summary>
    public Stack<Vector2> Path { get; private set; }

    /// <summary>
    /// Gets the final target position of the path.
    /// </summary>
    public Vector2 EndGoal { get; private set; }

    /// <summary>
    /// Gets the current position being worked towards.
    /// </summary>
    public Vector2 CurrentGoal { get => Path.Peek(); }

    /// <summary>
    /// Gets a value indicating whether there are any nodes left in the path.
    /// </summary>
    public bool IsFinished { get => Path.Count == 0; }
}