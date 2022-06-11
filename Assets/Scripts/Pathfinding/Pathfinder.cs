using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using BeanLib.References;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Component responsible for scheduling and running pathfinding tasks.
/// </summary>
public class Pathfinder : MonoBehaviour
{
    private readonly CancellationTokenSource cancellationSource = new CancellationTokenSource();

    private readonly ConcurrentQueue<QueuedPath> queuedPaths = new ConcurrentQueue<QueuedPath>();

    private readonly ConcurrentDictionary<(Vector2Int, Vector2Int), Stack<Vector2>> cachedPaths = new ConcurrentDictionary<(Vector2Int, Vector2Int), Stack<Vector2>>();

    private readonly ConcurrentDictionary<int, QueuedPath> latestQueuedPaths = new ConcurrentDictionary<int, QueuedPath>();

    private readonly Dictionary<Vector2Int, AStarTile> threadSafeTiles = new Dictionary<Vector2Int, AStarTile>();

    private readonly ConcurrentQueue<(QueuedPath PathInfo, Stack<Vector2> Path)> completedPathsQueue = new ConcurrentQueue<(QueuedPath PathInfo, Stack<Vector2> Path)>();

    private AStar currentPath = null;

    [SerializeField] private Tilemap tilemap;
    [SerializeField] private int maxCachedPaths = 200;
    [SerializeField] private int noPathsSleepTime = 100;

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

        if (cachedPaths.TryGetValue((startTilePos, endTilePos), out Stack<Vector2> result))
        {
            // run callback delegate
            onCompleteCallback(result);

            // exit
            return;
        }

        if (latestQueuedPaths.ContainsKey(callingObject.GetInstanceID()))
        {
            latestQueuedPaths[callingObject.GetInstanceID()].Cancelled = true;
        }

        QueuedPath pathInfo = new QueuedPath(startTilePos, endTilePos, callingObject, onCompleteCallback);

        queuedPaths.Enqueue(pathInfo);
        latestQueuedPaths[callingObject.GetInstanceID()] = pathInfo;

        enabled = true;
    }

    /// <summary>
    /// Cancels all pending paths for this object.
    /// </summary>
    /// <param name="caller">The caller requesting the cancel.</param>
    public void CancelObject(GameObject caller)
    {
        if (latestQueuedPaths.ContainsKey(caller.GetInstanceID()))
        {
            latestQueuedPaths[caller.GetInstanceID()].Cancelled = true;
        }
    }

    /// <summary>
    /// Tries to retrieve a request from the path request collection.
    /// </summary>
    /// <param name="requester">The object performing the request.</param>
    /// <param name="result">The result of the request.</param>
    /// <returns>True if a request was found, false if not.</returns>
    public bool TryGetRequest(GameObject requester, out (Vector2Int Start, Vector2Int End) result)
    {
        bool tryResult = latestQueuedPaths.TryGetValue(requester.GetInstanceID(), out QueuedPath foundValue);

        if (tryResult)
        {
            result = (foundValue.StartPos, foundValue.EndPos);
        }
        else
        {
            result = (Vector2Int.zero, Vector2Int.zero);
        }

        return tryResult;
    }

    private void Awake()
    {
        ReferenceStore.ReplaceReference(this);

        CollectTiles();

        BeginPathingThread();
    }

    private void Update()
    {
        while (completedPathsQueue.Count > 0)
        {
            if (completedPathsQueue.TryDequeue(out (QueuedPath PathInfo, Stack<Vector2> Path) result))
            {
                if (!result.PathInfo.Cancelled)
                {
                    result.PathInfo.Callback?.Invoke(result.Path);
                }
            }
        }
    }

    private void OnDestroy()
    {
        cancellationSource.Cancel();
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

    private void BeginPathingThread()
    {
        CancellationToken cancellationToken = cancellationSource.Token;

        Thread thread = new Thread(new ParameterizedThreadStart(ProcessPaths));
        thread.Start(cancellationToken);
    }

    private void PathFinished()
    {
        if (queuedPaths.TryDequeue(out QueuedPath path))
        {
            // cache path
            CachePath(path.StartPos, path.EndPos, currentPath.Path);

            // invoke callback
            completedPathsQueue.Enqueue((path, currentPath.Path));

            // reset path to null
            currentPath = null;

            // remove path from dictionary
            latestQueuedPaths.TryRemove(path.CallerID, out _);
        }
    }

    private void BeginNewPath()
    {
        if (queuedPaths.Count > 0)
        {
            // peek at first
            queuedPaths.TryPeek(out QueuedPath path);

            while (path.Cancelled)
            {
                // skip current peek
                queuedPaths.TryDequeue(out _);

                // peek at next
                queuedPaths.TryPeek(out path);
            }

            // create new path instance
            currentPath = new AStar(threadSafeTiles, path.StartPos, path.EndPos);

            // begin calculation
            currentPath.RunAlgorithm();
        }
        else
        {
            // sleep so not constantly calculating
            Thread.Sleep(noPathsSleepTime);
        }
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

    private void ProcessPaths(object param)
    {
        Debug.Log("Begin path thread");

        CancellationToken token = (CancellationToken)param;

        while (!token.IsCancellationRequested)
        {
            if (currentPath is null)
            {
                BeginNewPath();
            }

            if (currentPath != null && currentPath.FinishedCalculating)
            {
                PathFinished();
            }
        }

        Debug.Log("Exited path thread");
    }
}