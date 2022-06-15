using BeanLib.References;
using System.Collections;
using UnityEngine;

/// <summary>
/// Component responsible for reporting all damage recieved
/// </summary>
[RequireComponent(typeof(Damageable))]
public class TargetDummy : ReferenceResolvedBehaviour
{
    [BindComponent] private Damageable damageable;

    public override void Start()
    {
        base.Start();

        damageable.OnDamage += OnDamage;
    }

    private void OnDamage(float damage, GameObject source, DamageType type)
    {
        Debug.Log($"Target Dummy took {damage} damage of type {type} from object {source.name}");
    }
}
