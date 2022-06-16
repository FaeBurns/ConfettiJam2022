using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Component responsible for handling the <see cref="WalkerEnemy"/>.
/// </summary>
public class WalkerEnemy : EnemyBase
{
    [SerializeField] private int projectileThrowCount = 3;
    [SerializeField] private float projectileThrowInterval = 0.2f;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform projectileLaunchTransform;

    [Header("Attack preperation and aftermath")]
    [SerializeField] private float attackPrepareTime = 1f;

    [Header("Other")]
    [SerializeField] private List<GameObject> projectiles = new List<GameObject>();

    /// <summary>
    /// Gets the amount of projectiles currently out.
    /// </summary>
    private int ProjectileCount => projectiles.Count;

    /// <inheritdoc/>
    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        // halt if attacking
        if (State == EnemyState.WindUp || State == EnemyState.Attack)
        {
            Rb.MovePosition(Rb.position);
        }
    }

    /// <inheritdoc/>
    protected override IEnumerator DoAttack()
    {
        State = EnemyState.WindUp;

        yield return new WaitForSeconds(attackPrepareTime);

        State = EnemyState.Attack;

        float rotationAngle = Vector2.SignedAngle(Vector2.right, TargetPlayer.transform.position - projectileLaunchTransform.position);
        Quaternion rotation = Quaternion.Euler(0, 0, rotationAngle);

        while (ProjectileCount < projectileThrowCount)
        {
            yield return new WaitForSeconds(projectileThrowInterval);

            GameObject newObject = Instantiate(projectilePrefab, projectileLaunchTransform.position, rotation);

            BoomerangProjectile projectile = newObject.GetComponent<BoomerangProjectile>();

            projectile.PlayerObject = TargetPlayer;
            projectile.Finished += Projectile_Finished;

            projectiles.Add(newObject);
        }

        yield return new WaitWhile(() => ProjectileCount > 0);

        State = EnemyState.Idle;
    }

    private void Projectile_Finished(GameObject projectile)
    {
        projectiles.Remove(projectile);
    }
}