using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using BeanLib.References;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Component responsible for scheduling and running pathfinding tasks.
/// </summary>
public class Pathfinder : MonoBehaviour
{
    private readonly Dictionary<Vector2Int, AStarTile> threadSafeTiles = new Dictionary<Vector2Int, AStarTile>();
    private readonly ConcurrentDictionary<(Vector2Int, Vector2Int), Stack<Vector2>> cachedPaths = new ConcurrentDictionary<(Vector2Int, Vector2Int), Stack<Vector2>>();

    private readonly ConcurrentQueue<QueuedPath> queuedPaths = new ConcurrentQueue<QueuedPath>();
    private readonly List<PathTask> activePathTasks = new List<PathTask>();
    private readonly ConcurrentQueue<(QueuedPath PathInfo, Stack<Vector2> Path)> completedPathsQueue = new ConcurrentQueue<(QueuedPath PathInfo, Stack<Vector2> Path)>();

    [SerializeField] private Tilemap tilemap;
    [SerializeField] private int maxCachedPaths = 200;
    [SerializeField] private int workerThreadCount = 5;

    /// <summary>
    /// Delegate definition for path completion.
    /// </summary>
    /// <param name="path">The found path.</param>
    public delegate void OnPathFoundDelegate(Stack<Vector2> path);

    /// <summary>
    /// Adds the desired path to the pathfinding queue.
    /// </summary>
    /// <param name="startPos">The start position of the path.</param>
    /// <param name="endPos">The end position of the path.</param>
    /// <param name="callingObject">The object that called this function. Used to make sure that there aren't any delays in keeping up to date paths.</param>
    /// <param name="onCompleteCallback">Callback to be executed when the path is found.</param>
    public void FindPath(Vector2 startPos, Vector2 endPos, GameObject callingObject, OnPathFoundDelegate onCompleteCallback)
    {
        Vector2Int startTilePos = AStar.ConvertToTileSpace(startPos);
        Vector2Int endTilePos = AStar.ConvertToTileSpace(endPos);

        // if start and end positions are the same tile
        if (startTilePos == endTilePos)
        {
            // don't path, run callback delegate
            onCompleteCallback(null);
            return;
        }

        if (cachedPaths.TryGetValue((startTilePos, endTilePos), out Stack<Vector2> result))
        {
            // run callback delegate
            onCompleteCallback(result);

            // exit
            return;
        }

        QueuedPath pathInfo = new QueuedPath(startTilePos, endTilePos, callingObject, onCompleteCallback);

        queuedPaths.Enqueue(pathInfo);

        enabled = true;
    }

    private void Awake()
    {
        ReferenceStore.ReplaceReference(this);

        CollectTiles();

        BeginPathingThreads();
    }

    private void Update()
    {
        // loop through each path task
        foreach (PathTask task in activePathTasks)
        {
            // if there are paths to generate and task isn't in use
            if (queuedPaths.Count > 0 && !task.IsPathing)
            {
                if (queuedPaths.TryDequeue(out QueuedPath result))
                {
                    task.Path(result);
                }
            }
        }

        while (completedPathsQueue.Count > 0)
        {
            if (completedPathsQueue.TryDequeue(out (QueuedPath PathInfo, Stack<Vector2> Path) result))
            {
                if (result.PathInfo.Caller != null)
                {
                    result.PathInfo.Callback?.Invoke(result.Path);

                    // if path exists
                    if (result.Path != null)
                    {
                        //add to cache
                        CachePath(result.PathInfo.StartPos, result.PathInfo.EndPos, result.Path);
                    }
                }
            }
        }
    }

    private void OnDestroy()
    {
        foreach (PathTask task in activePathTasks)
        {
            task.Dispose();
        }
    }

    private void CollectTiles()
    {
        for (int x = tilemap.cellBounds.xMin; x < tilemap.cellBounds.xMax; x++)
        {
            for (int y = tilemap.cellBounds.yMin; y < tilemap.cellBounds.yMax; y++)
            {
                Vector3Int cellPosition = new Vector3Int(x, y, 0);
                AStarTile tile = tilemap.GetTile<AStarTile>(cellPosition);

                if (tile != null)
                {
                    threadSafeTiles.Add(new Vector2Int(x, y), tile);
                }
            }
        }
    }

    private void BeginPathingThreads()
    {
        for (int i = 0; i < workerThreadCount; i++)
        {
            PathTask pathTask = new PathTask(threadSafeTiles);
            pathTask.OnPathComplete += OnPathComplete;

            activePathTasks.Add(pathTask);
        }
    }

    private void OnPathComplete(QueuedPath pathInfo, Stack<Vector2> path)
    {
        completedPathsQueue.Enqueue((pathInfo, path));
    }

    private void CachePath(Vector2Int startPos, Vector2Int endPos, Stack<Vector2> path)
    {
        // trim if over max cached
        if (cachedPaths.Count > maxCachedPaths)
        {
            // first gets latest object so last might get first object???
            // no guarantee
            // this pretty much just gets a random element
            // might be first
            // might be last
            KeyValuePair<(Vector2Int, Vector2Int), Stack<Vector2>> element = cachedPaths.Last();

            // remove found element
            cachedPaths.TryRemove(element.Key, out _);
        }

        // if not cached already
        if (!cachedPaths.ContainsKey((startPos, endPos)))
        {
            cachedPaths.TryAdd((startPos, endPos), path);
        }
    }
}