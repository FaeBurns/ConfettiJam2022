using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RangedEnemy : EnemyBase
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

    protected override IEnumerator BeginAttack()
    {
        // wait for prepare time
        // lock rotation
        // loop if projectiles left to launch
        //  throw projectile
        //  wait projectileThrowInterval
        // wait until all projectiles back
        // wait for time
        // begin from start
        yield break;
    }
}