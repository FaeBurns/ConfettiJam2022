using BeanLib.References;
using System.Collections;
using System.Linq;
using UnityEngine;

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
    [SerializeField] private float windDownTime;
    [SerializeField] private float windingSpeed;
    [SerializeField] private float windSlowDistance = 1f;

    private EnemyState State
    {
        get => state;
        set
        {
            state = value;

            if (state == EnemyState.Idle)
            {
                if (triggerCountCheck.Objects.Count() > 0)
                {
                    TargetPlayer = triggerCountCheck.Objects.First();
                    OnPlayerEnterDetectionRange(TargetPlayer);
                }
            }
        }
    }

    public override void OnPlayerEnterDetectionRange(GameObject playerObject)
    {
        base.OnPlayerEnterDetectionRange(playerObject);
        State = EnemyState.FollowPath;
    }

    protected override void OnPathFinished()
    {
        base.OnPathFinished();
        State = EnemyState.Idle;
    }

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
                rb.MovePosition(rb.position + manualVel);
                break;
        }

        // Dash attack
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

            float distanceTravelled = Vector2.Distance(initialPosition, transform.position);
            float distanceClamped = Mathf.Clamp(distanceTravelled, 0, windSlowDistance);
            float distanceMult = 1 - distanceClamped;

            manualVel = distanceMult * windingSpeed * direction;

            yield return null;
        }

        yield return DoAttack();
    }

    private IEnumerator DoAttack()
    {
        manualVel = (TargetPlayer.transform.position - transform.position).normalized * dashSpeed;

        State = EnemyState.Attack;

        float endTime = Time.time + dashTime;

        trail.emitting = true;

        while (endTime > Time.time)
        {
            yield return null;
        }

        trail.emitting = false;

        State = EnemyState.Idle;
    }
}