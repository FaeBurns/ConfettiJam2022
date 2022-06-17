using UnityEngine;

/// <summary>
/// Holds information about a queued pathfinder path.
/// </summary>
public struct QueuedPath
{
    /// <summary>
    /// Initializes a new instance of the <see cref="QueuedPath"/> class.
    /// </summary>
    /// <param name="startPos"></param>
    /// <param name="endPos"></param>
    /// <param name="caller"></param>
    /// <param name="callback"></param>
    public QueuedPath(Vector2Int startPos, Vector2Int endPos, GameObject caller, Pathfinder.OnPathFoundDelegate callback)
    {
        StartPos = startPos;
        EndPos = endPos;
        Caller = caller;
        Callback = callback;
    }

    public Vector2Int StartPos { get; private set; }

    public Vector2Int EndPos { get; private set; }

    public GameObject Caller { get; private set; }

    public Pathfinder.OnPathFoundDelegate Callback { get; private set; }
}