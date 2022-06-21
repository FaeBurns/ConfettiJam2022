using System.Collections.Generic;
using BeanLib.References;
using UnityEngine;

/// <summary>
/// Component responsible for activating and deactiating a collectible based on how many enemies have died.
/// </summary>
public class CollectibleActivator : ReferenceResolvedBehaviour
{
    private readonly List<GameObject> targets = new List<GameObject>();

    /// <summary>
    /// Gets a value indicating whether there are any targets left.
    /// </summary>
    public bool CanUse => targets.Count == 0;

    /// <inheritdoc/>
    public override void Start()
    {
        base.Start();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject target = collision.gameObject;
        if (enabled && target.CompareTag("Enemy") && !targets.Contains(target))
        {
            Damageable damageable = target.GetComponent<Damageable>();

            if (damageable != null)
            {
                damageable.OnDeath += () => OnDeath(target);

                targets.Add(target);
            }
        }
    }

    private void Update()
    {
        // stop further updates and trigger events.
        enabled = false;
    }

    private void OnDeath(GameObject target)
    {
        targets.Remove(target);
    }
}