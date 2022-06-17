using System.Collections;
using UnityEngine;
using System.Threading;
using System.Collections.Generic;
using System;

public class PathTask : IDisposable
{
    const int PathTimeoutTime = 2000;
    const int NoPathSleepTime = 100;

    private readonly CancellationTokenSource cancellationSource;
    private readonly CancellationToken cancellationToken;

    private readonly Dictionary<Vector2Int, AStarTile> tilemap;

    private readonly object locker = new object();
    private AStar currentPath = null;
    private QueuedPath? pathInfo = null;

    private TimeoutTimer timeout = null;

    public event Action<QueuedPath, Stack<Vector2>> OnPathComplete;

    public bool IsPathing
    {
        get
        {
            lock (locker)
            {
                if (currentPath == null)
                {
                    return false;
                }

                return !currentPath.FinishedCalculating;
            }
        }
    }

    public PathTask(Dictionary<Vector2Int, AStarTile> tilemap)
    {
        cancellationSource = new CancellationTokenSource();
        cancellationToken = cancellationSource.Token;

        Thread thread = new Thread(new ThreadStart(MainLoop));
        thread.Start();
        this.tilemap = tilemap;
    }

    public void Dispose()
    {
        cancellationSource.Cancel();
    }

    public void Path(QueuedPath path)
    {
        lock (locker)
        {
            if (IsPathing)
            {
                Debug.LogWarning("Should not be trying to generate a path while one is still processing");
                return;
            }

            pathInfo = path;
            currentPath = new AStar(tilemap, path.StartPos, path.EndPos);

            timeout?.Cancel();

            timeout = new TimeoutTimer(PathTimeoutTime);
            timeout.TimeoutAction += Timeout_TimeoutAction;
            timeout.Begin();
        }
    }

    private void Timeout_TimeoutAction()
    {
        lock (locker)
        {
            if (currentPath != null)
            {
                Debug.Log("Timed out trying to generate path");

                OnPathComplete?.Invoke(pathInfo.Value, null);

                currentPath = null;
                pathInfo = null;
            }
        }
    }

    private void MainLoop()
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            bool shouldSleep = false;
            lock (locker)
            {
                // if there is no path to run
                if (currentPath == null)
                {
                    // skip rest of this loop
                    shouldSleep = true;
                }
            }

            if (shouldSleep)
            {
                // give delay so not constantly active
                Thread.Sleep(NoPathSleepTime);

                continue;
            }

            // run step of algorithm
            currentPath.RunAlgorithmStep();

            // check if finished pathing
            if (currentPath.FinishedCalculating)
            {
                PathCompleted();
            }
        }

        cancellationSource.Dispose();
    }

    private void PathCompleted()
    {
        lock (locker)
        {
            timeout?.Cancel();

            OnPathComplete?.Invoke(pathInfo.Value, currentPath.Path);

            currentPath = null;
            pathInfo = null;
        }
    }
}