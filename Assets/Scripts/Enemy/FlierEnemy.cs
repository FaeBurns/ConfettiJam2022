using System.Collections;
using BeanLib.References;
using UnityEngine;

/// <summary>
/// Component responsible for handling the <see cref="FlierEnemy"/>.
/// </summary>
public class FlierEnemy : EnemyBase
{
    private Vector2 manualVel;

    [BindComponent(Child = true)] private TrailRenderer trail;
    [BindComponent(Child = true)] private ContactDamageDealer contactDamage;

    [Header("Dash")]
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashTime;
    [SerializeField] private AnimationCurve windUpCurve;

    [Header("Attack preperation and aftermath")]
    [SerializeField] private float windUpTime;
    [SerializeField] private float windingSpeed;
    [SerializeField] private float windSlowDistance = 1f;

    [Header("Colliders")]
    [SerializeField] private Collider2D mainCollider;

    /// <inheritdoc/>
    protected override void OnPathFinished()
    {
        base.OnPathFinished();

        manualVel = Vector2.zero;
    }

    /// <inheritdoc/>
    protected override void OnStateChanged(EnemyState oldState, EnemyState newState)
    {
        base.OnStateChanged(oldState, newState);

        contactDamage.enabled = newState == EnemyState.Attack;
    }

    /// <inheritdoc/>
    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (State == EnemyState.WindUp || State == EnemyState.Attack)
        {
            Rb.MovePosition(Rb.position + manualVel);
        }
    }

    /// <inheritdoc/>
    protected override IEnumerator DoAttack()
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

        yield return Dash();
    }

    private IEnumerator Dash()
    {
        if(TargetPlayer == null)
        {
            State = EnemyState.Idle;
            yield break;
        }

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