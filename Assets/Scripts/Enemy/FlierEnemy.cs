using System.Collections;
using System.Collections.Generic;
using BeanLib.References;
using UnityEngine;

/// <summary>
/// Component responsible for handling the <see cref="FlierEnemy"/>.
/// </summary>
public class FlierEnemy : EnemyBase
{
    private Vector2 manualVel;

    [SerializeField] private EnemyState state = EnemyState.Idle;

    [BindComponent(Child = true)] private TrailRenderer trail;

    [Header("Attack")]
    [SerializeField] private float attackRadius;
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashTime;

    [Header("Attack preperation and aftermath")]
    [SerializeField] private float windUpTime;
    [SerializeField] private AnimationCurve windUpCurve;
    [SerializeField] private float windDownTime;
    [SerializeField] private float windingSpeed;
    [SerializeField] private float windSlowDistance = 1f;

    [Header("Colliders")]
    [SerializeField] private Collider2D mainCollider;

    [Header("Misc")]
    [SerializeField] private float knockbackRecieved;

    private EnemyState State
    {
        get => state;
        set
        {
            Debug.Log($"State Changed: {state}|{value}");
            state = value;
        }
    }

    /// <inheritdoc/>
    public override void OnPlayerEnterDetectionRange(GameObject playerObject)
    {
        switch (State)
        {
            case EnemyState.Idle:
            case EnemyState.DirectMove:
            case EnemyState.FollowPath:
                base.OnPlayerEnterDetectionRange(playerObject);
                State = EnemyState.FollowPath;
                break;
            default:
                break;
        }
    }

    /// <inheritdoc/>
    protected override void OnPathFinished()
    {
        base.OnPathFinished();
        State = EnemyState.Idle;

        manualVel = Vector2.zero;
    }

    /// <inheritdoc/>
    protected override void NoPath()
    {
        base.NoPath();
        OnPathFinished();
    }

    /// <inheritdoc/>
    protected override void Update()
    {
        base.Update();

        switch (State)
        {
            case EnemyState.Idle:
                // if player is still in range
                if (TargetPlayer != null)
                {
                    TryRepath();
                }

                break;
            case EnemyState.FollowPath:
                FollowPath();
                CheckAttackRadius();
                break;
        }
    }

    /// <inheritdoc/>
    protected override void BeginPath(Stack<Vector2> path, Vector2 endPos)
    {
        base.BeginPath(path, endPos);

        State = EnemyState.FollowPath;
    }

    /// <inheritdoc/>
    protected override void OnDamaged(float amount, GameObject source, DamageType damageType)
    {
        if (damageType == DamageType.Melee)
        {
            Vector2 direction = transform.position - source.transform.position;

            Rb.AddForce(direction * knockbackRecieved, ForceMode2D.Impulse);
        }
    }

    private void FixedUpdate()
    {
        switch (State)
        {
            case EnemyState.FollowPath:
                FollowPath();
                break;
            case EnemyState.WindUp:
            case EnemyState.WindDown:
            case EnemyState.Attack:
                Rb.MovePosition(Rb.position + manualVel);
                break;
            case EnemyState.Idle:
                Rb.MovePosition(Rb.position);
                break;
        }
    }

    private void CheckAttackRadius()
    {
        if (TargetPlayer == null)
        {
            return;
        }

        if (Vector2.Distance(transform.position, TargetPlayer.transform.position) <= attackRadius)
        {
            StartCoroutine(WindUpAttack());
        }
    }

    private IEnumerator WindUpAttack()
    {
        Vector2 initialPosition = transform.position;

        float endTime = Time.time + windUpTime;

        State = EnemyState.WindUp;

        while (endTime > Time.time)
        {
            if (TargetPlayer == null)
            {
                State = EnemyState.Idle;
                yield break;
            }

            Vector2 direction = (transform.position - TargetPlayer.transform.position).normalized;

            float distanceRemaining = windSlowDistance - Vector2.Distance(initialPosition, transform.position);

            // stop divide by zero
            float alpha = Mathf.Max(distanceRemaining / windSlowDistance, 0);

            float distanceMult = windUpCurve.Evaluate(alpha);

            manualVel = distanceMult * windingSpeed * direction;

            yield return null;
        }

        yield return DoAttack();
    }

    private IEnumerator DoAttack()
    {
        manualVel = (TargetPlayer.transform.position - transform.position).normalized * dashSpeed;

        float endTime = Time.time + dashTime;

        trail.emitting = true;
        mainCollider.isTrigger = true;
        State = EnemyState.Attack;

        yield return new WaitWhile(() => endTime > Time.time);

        trail.emitting = false;
        mainCollider.isTrigger = false;
        State = EnemyState.Idle;
    }
}