using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BeanLib.References;
using UnityEngine;

/// <summary>
/// A base class for common enemy behaviour.
/// </summary>
[RequireComponent(typeof(Damageable))]
[RequireComponent(typeof(Rigidbody2D))]
public class EnemyBase : ReferenceResolvedBehaviour
{
    private float scheduledRepathTime = 0f;
    private EnemyPathData? pathData;
    private PlayerPositionReporter targetPlayer;

    [AutoReference] private Pathfinder pathfinder;
    [BindComponent] private Rigidbody2D rb;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float repathTime = 1f;

    /// <summary>
    /// Moves the enemy to the target position.
    /// </summary>
    /// <param name="position">The position to move to.</param>
    public void MoveToPosition(Vector2 position)
    {
        pathfinder.FindPath(transform.position, position, gameObject, (path) => BeginPath(path, position));
    }

    /// <summary>
    /// Called when the player exits the detection range.
    /// </summary>
    public virtual void OnPlayerExitDetectionRange()
    {
        targetPlayer = null;
    }

    /// <summary>
    /// Called when the player enters the detection range.
    /// </summary>
    /// <param name="playerObject">The player.</param>
    public virtual void OnPlayerEnterDetectionRange(GameObject playerObject)
    {
        targetPlayer = playerObject.GetComponent<PlayerPositionReporter>();
        MoveToPosition(targetPlayer.transform.position);
    }

    /// <inheritdoc/>
    public override void Start()
    {
        base.Start();

        OnPlayerEnterDetectionRange(FindObjectOfType<PlayerMovement>().gameObject);
    }

    private void BeginPath(Stack<Vector2> path, Vector2 endPos)
    {
        if (path == null)
        {
            Debug.Log("Enemy path was null");
            return;
        }

        // if there is a currently active path
        // get closest point on path to start from instead of returning to beginning
        if (pathData != null)
        {
            int initialPathIndex = FindClosestPointOnPath(path.ToArray(), transform.position);

            // remove all up until the closest path point
            for (int i = 0; i < initialPathIndex; i++)
            {
                path.Pop();
            }
        }

        pathData = new EnemyPathData(path, endPos);

        scheduledRepathTime = Time.time + repathTime;
    }

    private void Update()
    {
        if (pathData != null)
        {
            Vector2 prevPosition = transform.position;

            foreach (Vector2 node in pathData.Value.Path)
            {
                Debug.DrawLine(prevPosition, node, Color.green);
                prevPosition = node;
            }
        }
    }

    private void FixedUpdate()
    {
        // exit if there is no path to follow
        if (this.pathData is null)
        {
            return;
        }

        // get normal reference to pathData
        // should probably use different var name but this hides the other one
        EnemyPathData pathData = this.pathData.Value;

        // if path is finished
        if (pathData.IsFinished)
        {
            // clear path and exit
            this.pathData = null;
            return;
        }

        // check if we should repath
        if (scheduledRepathTime <= Time.time)
        {
            // if there is a path request being made for this object
            if (pathfinder.TryGetRequest(gameObject, out (Vector2Int Start, Vector2Int End) result))
            {
                // if the path does not end up at the same position a recalculation would end up at
                if (result.End != AStar.ConvertToTileSpace(targetPlayer.ValidPathPosition))
                {
                    // requested path is not ideal, request one
                    MoveToPosition(targetPlayer.ValidPathPosition);
                }
            }
            else
            {
                // no request path found, request one
                MoveToPosition(targetPlayer.ValidPathPosition);
            }
        }

        // get normalized direction to next node
        Vector2 direction = pathData.CurrentGoal - (Vector2)transform.position;
        direction.Normalize();

        // get desired velocity
        Vector2 desiredVelocity = direction * moveSpeed;

        rb.AddForce(desiredVelocity -= rb.velocity, ForceMode2D.Force);

        // if we've reached the target node
        if (pathData.CurrentGoal.CloseEnough(transform.position))
        {
            // pop the node off the stack
            pathData.Path.Pop();
        }
    }

    private int FindClosestPointOnPath(Vector2[] path, Vector2 targetPosition)
    {
        int closestNodeIndex = -1;
        int secondClosestNodeIndex = -1;

        float closestDistance = float.MaxValue;
        float secondClosestDistance = float.MaxValue;

        // loop through all path nodes
        for (int i = 0; i < path.Length; i++)
        {
            // get current node
            Vector2 node = path[i];

            // get distance to current node
            float nodeDistance = Vector2.Distance(node, targetPosition);

            // check if this node is the new closest
            if (nodeDistance < closestDistance)
            {
                closestDistance = nodeDistance;
                closestNodeIndex = i;
            }

            // if further than closest distance but closer than second closest distance
            else if (nodeDistance < secondClosestDistance && nodeDistance > closestDistance)
            {
                secondClosestDistance = nodeDistance;
                secondClosestNodeIndex = i;
            }
        }

        int indexDiff = Mathf.Abs(closestNodeIndex - secondClosestNodeIndex);

        // if nodes are not directly next to each other
        // no idea if this can actually happen but just in case
        if (indexDiff > 1)
        {
            return closestNodeIndex;
        }

        // if closest is the first, go to first anyway just to be safe
        // in this case, secondClosestNodeIndex is always going to be further along
        if (closestNodeIndex == 0)
        {
            return closestNodeIndex;
        }

        Debug.Assert(closestNodeIndex != secondClosestNodeIndex);

        // return highest index
        return Mathf.Max(closestNodeIndex, secondClosestNodeIndex);
    }
}