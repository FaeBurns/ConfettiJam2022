using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeanLib.References;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Component responsible for scheduling and running pathfinding tasks.
/// </summary>
public class Pathfinder : MonoBehaviour
{
    private readonly Queue<QueuedPath> queuedPaths = new Queue<QueuedPath>();

    private readonly Dictionary<(Vector2Int, Vector2Int), Stack<Vector2>> cachedPaths = new Dictionary<(Vector2Int, Vector2Int), Stack<Vector2>>();

    private readonly Dictionary<GameObject, QueuedPath> referencedQueuedPaths = new Dictionary<GameObject, QueuedPath>();

    private GameObject currentPathCaller = null;
    private AStar currentPath = null;

    [SerializeField] private Tilemap tilemap;
    [SerializeField] private int maxStepsPerEnumeratedAlgorithmCycle = 200;
    [SerializeField] private int maxCachedPaths = 200;

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

        if (referencedQueuedPaths.ContainsKey(callingObject))
        {
            referencedQueuedPaths[callingObject].Cancelled = true;
        }

        QueuedPath pathInfo = new QueuedPath(startTilePos, endTilePos, callingObject, onCompleteCallback);

        queuedPaths.Enqueue(pathInfo);
        referencedQueuedPaths[callingObject] = pathInfo;

        enabled = true;
    }

    /// <summary>
    /// Tries to retrieve a request from the path request collection.
    /// </summary>
    /// <param name="requester">The object performing the request.</param>
    /// <param name="result">The result of the request.</param>
    /// <returns>True if a request was found, false if not.</returns>
    public bool TryGetRequest(GameObject requester, out (Vector2Int Start, Vector2Int End) result)
    {
        bool tryResult = referencedQueuedPaths.TryGetValue(requester, out QueuedPath foundValue);

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
    }

    private void Update()
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

    private void PathFinished()
    {
        // get value
        QueuedPath path = queuedPaths.Dequeue();

        // cache path
        CachePath(path.StartPos, path.EndPos, currentPath.Path);

        // invoke callback
        path.Callback?.Invoke(currentPath.Path);

        // reset path to null
        currentPath = null;

        // remove path from dictionary
        referencedQueuedPaths.Remove(currentPathCaller);
        currentPathCaller = null;
    }

    private void BeginNewPath()
    {
        if (queuedPaths.Count > 0)
        {
            enabled = true;

            // peek at first
            QueuedPath path = queuedPaths.Peek();

            while (path.Cancelled)
            {
                // skip current peek
                queuedPaths.Dequeue();

                // peek at next
                path = queuedPaths.Peek();
            }

            // create new path instance
            currentPath = new AStar(tilemap, path.StartPos, path.EndPos);

            // set known caller
            currentPathCaller = path.Caller;

            // begin calculation
            StartCoroutine(currentPath.RunAlgorithmEnumerated(maxStepsPerEnumeratedAlgorithmCycle));
        }
        else
        {
            enabled = false;
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
            cachedPaths.Remove(element.Key);
        }

        // if not cached already
        if (!cachedPaths.ContainsKey((startPos, endPos)))
        {
            cachedPaths.Add((startPos, endPos), path);
        }
    }
}