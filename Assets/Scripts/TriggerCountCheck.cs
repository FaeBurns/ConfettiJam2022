using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Component responsible for keeping track of all colliders it is overlapping with.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class TriggerCountCheck : MonoBehaviour
{
    private readonly Dictionary<Collider2D, int> colliders = new Dictionary<Collider2D, int>();
    private readonly Dictionary<GameObject, int> objects = new Dictionary<GameObject, int>();
    private readonly HashSet<string> whitelist = new HashSet<string>();

    [SerializeField] private string[] tagWhitelist;

    public event Action<Collider2D> OnColliderEntry;
    public event Action<Collider2D> OnColliderExit;
    public event Action<GameObject> OnObjectEntry;
    public event Action<GameObject> OnObjectExit;

    /// <summary>
    /// Gets a collection of all the colliders present in the trigger.
    /// </summary>
    public IEnumerable<Collider2D> Colliders { get => colliders.Keys; }

    /// <summary>
    /// Gets a collection of all the objects present in the trigger.
    /// </summary>
    public IEnumerable<GameObject> Objects { get => objects.Keys; }

    private void Start()
    {
        for (int i = 0; i < tagWhitelist.Length; i++)
        {
            whitelist.Add(tagWhitelist[i]);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log($"{collision.gameObject.name} entered");

        if (!tagWhitelist.Contains(collision.tag))
        {
            return;
        }

        if (colliders.ContainsKey(collision))
        {
            colliders[collision]++;
        }
        else
        {
            colliders[collision] = 1;
            OnColliderEntry?.Invoke(collision);
        }

        if (objects.ContainsKey(collision.gameObject))
        {
            objects[collision.gameObject]++;
        }
        else
        {
            objects[collision.gameObject] = 1;
            OnObjectEntry?.Invoke(collision.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log($"{collision.gameObject.name} exited");

        if (colliders.ContainsKey(collision))
        {
            colliders[collision]--;

            if (colliders[collision] == 0)
            {
                colliders.Remove(collision);
                OnColliderExit?.Invoke(collision);
            }
        }

        if (objects.ContainsKey(collision.gameObject))
        {
            objects[collision.gameObject]--;

            if (objects[collision.gameObject] == 0)
            {
                objects.Remove(collision.gameObject);
                OnObjectExit?.Invoke(collision.gameObject);
            }
        }
    }
}