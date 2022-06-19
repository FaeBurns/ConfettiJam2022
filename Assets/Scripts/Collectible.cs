using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeanLib.References;
using UnityEngine;

/// <summary>
/// Component responsible for collecting items.
/// </summary>
public class Collectible : ReferenceResolvedBehaviour
{
    [AutoReference] private CollectibleGoalManager goalManager;

    [Min(1f)]
    [SerializeField] private float scaleMagnitude;
    [SerializeField] private float timescale = 1f;

    private event Action<Sprite> OnCollect;

    /// <inheritdoc/>
    public override void Start()
    {
        base.Start();

        OnCollect += goalManager.OnCollect;
    }

    private void Update()
    {
        transform.localScale = Vector3.Lerp(Vector3.one, new Vector3(scaleMagnitude, scaleMagnitude, scaleMagnitude), Mathf.Sin(Time.time * timescale));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            OnCollect(GetComponentInChildren<SpriteRenderer>().sprite);
            Destroy(gameObject);
        }
    }
}
