using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// Component responsible for keeping track of all colliders it is overlapping with.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class TriggerCountCheck : MonoBehaviour
{
    private readonly HashSet<Collider2D> colliders = new HashSet<Collider2D>();
    private readonly HashSet<GameObject> objects = new HashSet<GameObject>();

    /// <summary>
    /// Gets a collection of all the colliders present in the trigger.
    /// </summary>
    public IEnumerable<Collider2D> Colliders { get => colliders; }

    /// <summary>
    /// Gets a collection of all the objects present in the trigger.
    /// </summary>
    public IEnumerable<GameObject> Objects { get => objects; }

    private void Start()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!Colliders.Contains(collision))
        {
            colliders.Add(collision);
        }

        if (!Objects.Contains(collision.gameObject))
        {
            objects.Add(collision.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (Colliders.Contains(collision))
        {
            colliders.Remove(collision);
        }

        if (Objects.Contains(collision.gameObject))
        {
            objects.Remove(collision.gameObject);
        }
    }
}