using System.Collections.Generic;
using BeanLib.References;
using UnityEngine;

/// <summary>
/// A base class for common enemy behaviour.
/// </summary>
[RequireComponent(typeof(Damageable))]
[RequireComponent(typeof(Rigidbody2D))]
public abstract class EnemyBase : ReferenceResolvedBehaviour
{
    private Vector2 cachedLastPlayerPosition = Vector2.zero;
    private float scheduledRepathTime = 0f;
    private EnemyPathData? pathData;
    private PlayerPositionReporter targetPlayerPositionReporter;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float repathTime = 1f;

    /// <summary>
    /// Gets or Sets the <see cref="global::Pathfinder"/> component reference.
    /// </summary>
    [AutoReference] protected Pathfinder Pathfinder { get; set; }

    /// <summary>
    /// Gets or Sets the <see cref="Rigidbody2D"/> component reference.
    /// </summary>
    [BindComponent] protected Rigidbody2D Rb { get; set; }

    /// <summary>
    /// Gets or Sets the <see cref="global::TriggerCountCheck"/> component reference.
    /// </summary>
    [BindComponent(Child = true)] protected TriggerCountCheck TriggerCountCheck { get; set; }

    /// <summary>
    /// Gets or Sets the player <see cref="GameObject"/> in targeting range.
    /// </summary>
    protected GameObject TargetPlayer
    {
        get
        {
            if (targetPlayerPositionReporter != null)
            {
                return targetPlayerPositionReporter.gameObject;
            }

            return null;
        }

        set
        {
            if (value != null)
            {
                targetPlayerPositionReporter = value.GetComponent<PlayerPositionReporter>();
            }
            else
            {
                targetPlayerPositionReporter = null;
            }
        }
    }

    private Vector2 LastPlayerPosition
    {
        get
        {
            if (targetPlayerPositionReporter != null)
            {
                cachedLastPlayerPosition = targetPlayerPositionReporter.ValidPathPosition;
            }

            return cachedLastPlayerPosition;
        }
    }

    /// <summary>
    /// Moves the enemy to the target position.
    /// </summary>
    /// <param name="position">The position to move to.</param>
    public void MoveToPosition(Vector2 position)
    {
        Pathfinder.FindPath(transform.position, position, gameObject, (path) => BeginPath(path, position));
    }

    /// <summary>
    /// Called when the player exits the detection range.
    /// </summary>
    /// <param name="playerObject">The player object leaving range.</param>
    public virtual void OnPlayerExitDetectionRange(GameObject playerObject)
    {
        targetPlayerPositionReporter = null;
    }

    /// <summary>
    /// Called when the player enters the detection range.
    /// </summary>
    /// <param name="playerObject">The player object entering range.</param>
    public virtual void OnPlayerEnterDetectionRange(GameObject playerObject)
    {
        targetPlayerPositionReporter = playerObject.GetComponent<PlayerPositionReporter>();
        MoveToPosition(targetPlayerPositionReporter.transform.position);
    }

    /// <inheritdoc/>
    public override void Start()
    {
        base.Start();

        TriggerCountCheck.OnObjectEntry += OnPlayerEnterDetectionRange;
        TriggerCountCheck.OnObjectExit += OnPlayerExitDetectionRange;

        Damageable damageable = GetComponent<Damageable>();

        damageable.OnDeath += OnDeath;
        damageable.OnDamage += OnDamaged;
    }

    /// <summary>
    /// Called when the path has finished calculating.
    /// </summary>
    protected virtual void OnPathFinished() { }

    /// <summary>
    /// Called when there is no path to follow.
    /// </summary>
    protected virtual void NoPath() { }

    /// <summary>
    /// Unity Update Message.
    /// </summary>
    protected virtual void Update()
    {
        DrawPathDebug();
    }

    /// <summary>
    /// Follow the calculated path.
    /// Exits if no path is available.
    /// </summary>
    protected void FollowPath()
    {
        // exit if there is no path to follow
        if (!this.pathData.HasValue)
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

        TryRepath();

        // get normalized direction to next node
        Vector2 direction = pathData.CurrentGoal - (Vector2)transform.position;
        direction.Normalize();

        // get desired velocity
        Vector2 desiredVelocity = direction * moveSpeed;

        Rb.AddForce(desiredVelocity -= Rb.velocity, ForceMode2D.Force);

        // if we've reached the target node
        if (pathData.CurrentGoal.CloseEnough(transform.position))
        {
            // pop the node off the stack
            pathData.Path.Pop();

            // if there are no more nodes left
            if (pathData.Path.Count == 0)
            {
                // clear path data
                this.pathData = null;

                OnPathFinished();
            }
        }
    }

    /// <summary>
    /// Checks to see if a repath is due, and performs one if required.
    /// </summary>
    protected void TryRepath()
    {
        // check if we should repath
        if (scheduledRepathTime <= Time.time)
        {
            // if there is a path request being made for this object
            if (Pathfinder.TryGetRequest(gameObject, out (Vector2Int Start, Vector2Int End) result))
            {
                // if the path does not end up at the same position a recalculation would end up at
                if (result.End != AStar.ConvertToTileSpace(LastPlayerPosition))
                {
                    // requested path is not ideal, request one
                    MoveToPosition(LastPlayerPosition);
                }
            }
            else
            {
                // no request path found, request one
                MoveToPosition(LastPlayerPosition);
            }
        }
    }

    /// <summary>
    /// Sets the enemy to follow along the path.
    /// </summary>
    /// <param name="path">The path to follow.</param>
    /// <param name="endPos">The end position of the path.</param>
    protected virtual void BeginPath(Stack<Vector2> path, Vector2 endPos)
    {
        if (path == null)
        {
            Debug.Log("Enemy path was null");
            NoPath();
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

    /// <summary>
    /// Called when this enemy takes any damage.
    /// </summary>
    /// <param name="amount">The amount of damage taken.</param>
    /// <param name="source">The object that caused the damage.</param>
    /// <param name="damageType">The type of damage dealt.</param>
    protected abstract void OnDamaged(float amount, GameObject source, DamageType damageType);

    private void DrawPathDebug()
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

    private void OnDeath()
    {
        Pathfinder.CancelObject(gameObject);
        Destroy(gameObject);
    }
}