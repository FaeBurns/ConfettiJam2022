using System.Collections;
using System.Linq;
using BeanLib.References;
using UnityEngine;

/// <summary>
/// Component responsible for handling the <see cref="FlierEnemy"/>.
/// </summary>
public class FlierEnemy : EnemyBase
{
    private Vector2 manualVel;

    [SerializeField]
    private EnemyState state = EnemyState.Idle;

    [Header("Visuals")]
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

    private EnemyState State
    {
        get => state;
        set
        {
            state = value;

            if (state == EnemyState.Idle)
            {
                if (TriggerCountCheck.Objects.Count() > 0)
                {
                    TargetPlayer = TriggerCountCheck.Objects.Where((obj) => obj.GetComponents<PlayerPositionReporter>() != null).First();
                    OnPlayerEnterDetectionRange(TargetPlayer);
                }
            }
        }
    }

    /// <inheritdoc/>
    public override void OnPlayerEnterDetectionRange(GameObject playerObject)
    {
        base.OnPlayerEnterDetectionRange(playerObject);
        State = EnemyState.FollowPath;
    }

    /// <inheritdoc/>
    protected override void OnPathFinished()
    {
        base.OnPathFinished();
        State = EnemyState.Idle;
    }

    /// <inheritdoc/>
    protected override void Update()
    {
        base.Update();

        switch (State)
        {
            case EnemyState.FollowPath:
                FollowPath();
                CheckAttackRadius();
                break;
        }
    }

    private void FixedUpdate()
    {
        switch (State)
        {
            case EnemyState.FollowPath:
                FollowPath();
                break;
            case EnemyState.Windup:
            case EnemyState.WindDown:
            case EnemyState.Attack:
                Rb.MovePosition(Rb.position + manualVel);
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

        State = EnemyState.Windup;

        while (endTime > Time.time)
        {
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