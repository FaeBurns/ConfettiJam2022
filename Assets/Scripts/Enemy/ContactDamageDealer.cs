using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// Component responsible for dealing contact damage.
/// </summary>
public class ContactDamageDealer : MonoBehaviour
{
    [SerializeField] private float damage;
    [SerializeField] private string[] tagFilter = { "Player" };
    [SerializeField] private ContactDamageSource desiredSource;

    [Tooltip("The object to blame for the damage")]
    [SerializeField] private GameObject blameObject;

    [SerializeField] private DamageType damageType;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (Validate(collision.collider, ContactDamageSource.Collide))
        {
            DealDamage(collision.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (Validate(collision, ContactDamageSource.Trigger))
        {
            DealDamage(collision.gameObject);
        }
    }

    private bool Validate(Collider2D collider, ContactDamageSource source)
    {
        if (!tagFilter.Contains(collider.gameObject.tag))
        {
            return false;
        }

        if (desiredSource == (ContactDamageSource.Collide | ContactDamageSource.Trigger))
        {
            return true;
        }

        return (source & desiredSource) == source;
    }

    private void DealDamage(GameObject target)
    {
        Damageable damageable = target.GetComponent<Damageable>();

        if (damageable != null)
        {
            damageable.DealDamage(damage, blameObject, damageType);
        }
    }
}