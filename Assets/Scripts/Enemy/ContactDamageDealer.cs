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


    [SerializeField] private DamageType damageType;

    /// <summary>
    /// Gets or sets the object to blame for the damage.
    /// </summary>
    [field: Tooltip("The object to blame for the damage")]
    [field: SerializeField] 
    public GameObject BlameObject { get; set; }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (Validate(collision.collider))
        {
            DealDamage(collision.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (Validate(collision))
        {
            DealDamage(collision.gameObject);
        }
    }

    private bool Validate(Collider2D collider)
    {
        return tagFilter.Contains(collider.gameObject.tag);
    }

    private void DealDamage(GameObject target)
    {
        Damageable damageable = target.GetComponent<Damageable>();

        if (damageable != null)
        {
            damageable.DealDamage(damage, BlameObject, damageType);
        }
    }
}